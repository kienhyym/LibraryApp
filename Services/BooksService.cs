using LibraryApp.Common;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Book;
using LibraryApp.ViewModels.Home;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class BooksService : IBooksService
{
    private readonly LibDbContext _context;

    public BooksService(
        LibDbContext context)
    {
        _context = context;
    }

    #region Book List

    public async Task<BookListViewModel> GetBooksAsync(
        BookFilterViewModel filter)
    {
        var model = new BookListViewModel
        {
            Filter = filter
        };

        model.Categories = await GetCategoriesAsync();

        model.Authors = await GetAuthorsAsync();
        model.TotalBooks = await _context.Books.CountAsync();
        var query = _context.Books

            .AsNoTracking()

            .Include(x => x.Author)

            .Include(x => x.Category)

            .AsQueryable();

        // ==========================
        // Keyword
        // ==========================

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            string keyword = filter.Keyword.Trim();

            query = query.Where(x =>

                x.Title.Contains(keyword)

                ||

                x.Author.AuthorName.Contains(keyword)

                ||

                x.Category.CategoryName.Contains(keyword)

                ||

                (x.Publisher != null &&
                 x.Publisher.Contains(keyword)));
        }

        // ==========================
        // Category
        // ==========================

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == filter.CategoryId);
        }

        // ==========================
        // Author
        // ==========================

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(x =>
                x.AuthorId == filter.AuthorId);
        }

        // ==========================
        // Sort
        // ==========================

        query = filter.SortBy switch
        {
            "title"

                => query.OrderBy(x => x.Title),

            "popular"

                => query.OrderByDescending(x =>
                    x.Borrowrecorddetails.Count),

            "available"

                => query.OrderByDescending(x =>
                    x.AvailableQuantity),

            _

                => query.OrderByDescending(x =>
                    x.CreatedAt)
        };

        // ==========================
        // Projection
        // ==========================

        var books = query.Select(x =>

            new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable
            });

        model.Books =
            await PaginatedList<BookCardViewModel>.CreateAsync(

                books,

                filter.Page,

                filter.PageSize);

        return model;
    }

    #endregion
        #region Book Detail

    public async Task<BookDetailViewModel?> GetBookDetailAsync(
        int bookId)
    {
        return await _context.Books

            .AsNoTracking()

            .Where(x => x.BookId == bookId)

            .Select(x => new BookDetailViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                Publisher = x.Publisher,

                PublishYear = x.PublicationYear,

                Quantity = x.Quantity,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable,

                Description = x.BookDescription,

            })

            .FirstOrDefaultAsync();
    }

    #endregion

    #region Dropdown

    public async Task<List<SelectListItem>> GetCategoriesAsync()
    {
        var items = await _context.Categories

            .AsNoTracking()

            .OrderBy(x => x.CategoryName)

            .Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),

                Text = x.CategoryName
            })

            .ToListAsync();

        items.Insert(0, new SelectListItem
        {
            Value = "",

            Text = "-- Tất cả thể loại --"
        });

        return items;
    }

    public async Task<List<SelectListItem>> GetAuthorsAsync()
    {
        var items = await _context.Authors

            .AsNoTracking()

            .OrderBy(x => x.AuthorName)

            .Select(x => new SelectListItem
            {
                Value = x.AuthorId.ToString(),

                Text = x.AuthorName
            })

            .ToListAsync();

        items.Insert(0, new SelectListItem
        {
            Value = "",

            Text = "-- Tất cả tác giả --"
        });

        return items;
    }

    #endregion
        #region Related Books

    public async Task<List<BookCardViewModel>> GetRelatedBooksAsync(
        int bookId)
    {
        // ===================================
        // Lấy thông tin sách hiện tại
        // ===================================

        var currentBook = await _context.Books

            .AsNoTracking()

            .Where(x => x.BookId == bookId)

            .Select(x => new
            {
                x.CategoryId,
                x.AuthorId
            })

            .FirstOrDefaultAsync();

        if (currentBook == null)
        {
            return new List<BookCardViewModel>();
        }

        // ===================================
        // Cùng thể loại
        // ===================================

        var relatedBooks = await _context.Books

            .AsNoTracking()

            .Where(x =>

                x.BookId != bookId

                &&

                x.CategoryId == currentBook.CategoryId)

            .OrderByDescending(x => x.CreatedAt)

            .Take(6)

            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable
            })

            .ToListAsync();

        // ===================================
        // Nếu chưa đủ thì lấy cùng tác giả
        // ===================================

        if (relatedBooks.Count < 6)
        {
            var existedIds = relatedBooks
                .Select(x => x.BookId)
                .ToList();

            existedIds.Add(bookId);

            var authorBooks = await _context.Books

                .AsNoTracking()

                .Where(x =>

                    x.AuthorId == currentBook.AuthorId

                    &&

                    !existedIds.Contains(x.BookId))

                .OrderByDescending(x => x.CreatedAt)

                .Take(6 - relatedBooks.Count)

                .Select(x => new BookCardViewModel
                {
                    BookId = x.BookId,

                    Title = x.Title,

                    CoverImage = x.CoverImage,

                    AuthorName = x.Author.AuthorName,

                    CategoryName = x.Category.CategoryName,

                    AvailableQuantity = x.AvailableQuantity,

                    IsAvailable = x.IsAvailable
                })

                .ToListAsync();

            relatedBooks.AddRange(authorBooks);
        }

        // ===================================
        // Nếu vẫn chưa đủ thì lấy sách mới
        // ===================================

        if (relatedBooks.Count < 6)
        {
            var existedIds = relatedBooks
                .Select(x => x.BookId)
                .ToList();

            existedIds.Add(bookId);

            var newestBooks = await _context.Books

                .AsNoTracking()

                .Where(x =>

                    !existedIds.Contains(x.BookId))

                .OrderByDescending(x => x.CreatedAt)

                .Take(6 - relatedBooks.Count)

                .Select(x => new BookCardViewModel
                {
                    BookId = x.BookId,

                    Title = x.Title,

                    CoverImage = x.CoverImage,

                    AuthorName = x.Author.AuthorName,

                    CategoryName = x.Category.CategoryName,

                    AvailableQuantity = x.AvailableQuantity,

                    IsAvailable = x.IsAvailable
                })

                .ToListAsync();

            relatedBooks.AddRange(newestBooks);
        }

        return relatedBooks;
    }

    #endregion
}