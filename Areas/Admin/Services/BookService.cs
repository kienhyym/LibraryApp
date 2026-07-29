using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Helpers;

namespace LibraryApp.Services;

public class BookService : IBookService
{
    private readonly LibDbContext _context;

    private readonly IWebHostEnvironment _environment;

    public BookService(
        LibDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    #region Get Paged

    public async Task<PaginatedList<BookViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize)
    {
        var query = _context.Books
            .Include(x => x.Author)
            .Include(x => x.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>
                x.Title.Contains(keyword));
        }

        var result = query

            .OrderByDescending(x => x.CreatedAt)

            .Select(x => new BookViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                Quantity = x.Quantity,

                AvailableQuantity = x.AvailableQuantity,

                CoverImage = x.CoverImage,

                CreatedAt = x.CreatedAt
            });

        return await PaginatedList<BookViewModel>

            .CreateAsync(result, page, pageSize);
    }

    #endregion

    #region Create Model

    public async Task<BookViewModel> GetCreateModelAsync()
    {
        var model = new BookViewModel();

        await LoadDropdownDataAsync(model);

        return model;
    }
    #endregion

    #region Edit Model

    public async Task<BookViewModel?> GetEditModelAsync(int id)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.BookId == id);

        if (book == null)
            return null;

        var model = new BookViewModel
        {
            BookId = book.BookId,

            Title = book.Title,

            AuthorId = book.AuthorId,

            CategoryId = book.CategoryId,

            Publisher = book.Publisher,

            PublicationYear = book.PublicationYear,

            Quantity = book.Quantity,

            AvailableQuantity = book.AvailableQuantity,

            BookDescription = book.BookDescription,

            CoverImage = book.CoverImage,

            IsAvailable = book.IsAvailable
        };

        await LoadDropdownDataAsync(model);

        return model;
    }

    #endregion

    #region Get By Id

    public async Task<BookViewModel?> GetByIdAsync(int id)
    {
        return await GetEditModelAsync(id);
    }

    #endregion

    #region Create

    public async Task CreateAsync(BookViewModel model)
    {
        var imagePath = await SaveImageAsync(
     model.CoverImageFile,
     model.Title);

        var book = new Book
        {
            Title = model.Title.Trim(),

            AuthorId = model.AuthorId,

            CategoryId = model.CategoryId,

            Publisher = model.Publisher?.Trim(),

            PublicationYear = model.PublicationYear,

            Quantity = model.Quantity,

            AvailableQuantity = model.Quantity,

            BookDescription = model.BookDescription?.Trim(),

            CoverImage = imagePath,

            IsAvailable = true
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Update

    public async Task UpdateAsync(BookViewModel model)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.BookId == model.BookId);

        if (book == null)
            throw new Exception("Không tìm thấy sách.");

        // Lưu số lượng cũ
        var oldQuantity = book.Quantity;
        var newQuantity = model.Quantity;

        // Nếu tăng số lượng
        if (newQuantity > oldQuantity)
        {
            var increase = newQuantity - oldQuantity;

            book.AvailableQuantity += increase;
        }
        // Nếu giảm số lượng
        else if (newQuantity < oldQuantity)
        {
            var decrease = oldQuantity - newQuantity;

            if (book.AvailableQuantity < decrease)
            {
                throw new Exception(
                    "Không thể giảm số lượng vì số sách đang được mượn.");
            }

            book.AvailableQuantity -= decrease;
        }

        // Cập nhật thông tin
        book.Title = model.Title.Trim();

        book.AuthorId = model.AuthorId;

        book.CategoryId = model.CategoryId;

        book.Publisher = model.Publisher?.Trim();

        book.PublicationYear = model.PublicationYear;

        book.Quantity = newQuantity;

        book.BookDescription = model.BookDescription?.Trim();

        // Nếu chọn ảnh mới
        if (model.CoverImageFile != null)
        {
            // Xóa ảnh cũ
            if (!string.IsNullOrWhiteSpace(book.CoverImage))
            {
                var oldImagePath = Path.Combine(
                    _environment.WebRootPath,
                    book.CoverImage.TrimStart('/'));

                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            // Lưu ảnh mới
           book.CoverImage =
    await SaveImageAsync(
        model.CoverImageFile,
        model.Title);
        }

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Delete

    public async Task DeleteAsync(int id)
    {
        var book = await _context.Books
            .FirstOrDefaultAsync(x => x.BookId == id);

        if (book == null)
            return;

        var borrowed = await _context.Borrowrecorddetails
            .AnyAsync(x => x.BookId == id);

        if (borrowed)
        {
            throw new InvalidOperationException(
                "Sách đã có phiếu mượn, không thể xóa.");
        }

        // Xóa ảnh
        if (!string.IsNullOrWhiteSpace(book.CoverImage))
        {
            var imagePath = Path.Combine(
                _environment.WebRootPath,
                book.CoverImage.TrimStart('/'));

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        _context.Books.Remove(book);

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Validation

    public async Task<bool> BookExistsByTitleAsync(string title)
    {
        title = title.Trim();

        return await _context.Books
            .AnyAsync(x => x.Title == title);
    }

    public async Task<bool> BookExistsByTitleForUpdateAsync(
        string title,
        int bookId)
    {
        title = title.Trim();

        return await _context.Books
            .AnyAsync(x =>
                x.Title == title &&
                x.BookId != bookId);
    }

    #endregion

    #region Dropdown

    private async Task<List<SelectListItem>> GetAuthorSelectListAsync()
    {
        return await _context.Authors

            .OrderBy(x => x.AuthorName)

            .Select(x => new SelectListItem
            {
                Value = x.AuthorId.ToString(),

                Text = x.AuthorName
            })

            .ToListAsync();
    }

    private async Task<List<SelectListItem>> GetCategorySelectListAsync()
    {
        return await _context.Categories

            .OrderBy(x => x.CategoryName)

            .Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),

                Text = x.CategoryName
            })

            .ToListAsync();
    }

    #endregion

    #region Upload Image

    private async Task<string?> SaveImageAsync(
    IFormFile? file,
    string bookTitle)
    {
        if (file == null || file.Length == 0)
            return null;

        var folder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "books");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var slug = SlugHelper.Generate(bookTitle);

        var fileName =
            $"{slug}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLower()}";

        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);

        await file.CopyToAsync(stream);

        return $"/uploads/books/{fileName}";
    }

    #endregion

    public async Task LoadDropdownDataAsync(BookViewModel model)
    {
        model.Authors = await GetAuthorSelectListAsync();

        model.Categories = await GetCategorySelectListAsync();
    }
}