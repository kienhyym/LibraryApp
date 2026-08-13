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

    public BooksService(LibDbContext context)
    {
        _context = context;
    }

    #region Book List

    public async Task<BookListViewModel> GetBooksAsync(
        int? accountId,
        BookFilterViewModel filter)
    {
        var model = new BookListViewModel
        {
            Filter = filter
        };

        model.Categories = await GetCategoriesAsync();

        model.Authors = await GetAuthorsAsync();

        model.TotalBooks = await _context.Books.CountAsync();

        // ==========================================
        // Lấy ResidentId
        // ==========================================

        int? residentId = null;

        if (accountId.HasValue)
        {
            residentId = await _context.Residents
                .Where(x => x.AccountId == accountId.Value)
                .Select(x => (int?)x.ResidentId)
                .FirstOrDefaultAsync();
        }

        // ==========================================
        // Danh sách sách yêu thích
        // ==========================================

        var favoriteBookIds = residentId.HasValue
            ? await _context.Favoritebooks
                .Where(x => x.ResidentId == residentId.Value)
                .Select(x => x.BookId)
                .ToListAsync()
            : new List<int>();

        // ==========================================
        // Query Books
        // ==========================================

        var query = _context.Books
            .AsNoTracking()
            .Include(x => x.Authors)
            .Include(x => x.Category)
            .AsQueryable();

        // ==========================================
        // Keyword
        // ==========================================

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            string keyword = filter.Keyword.Trim();

            query = query.Where(x =>
                x.Title.Contains(keyword)

                ||

                x.Authors.Any(a =>
                    a.AuthorName.Contains(keyword))

                ||

                x.Category.CategoryName.Contains(keyword)

                ||

                (
                    x.Publisher != null &&
                    x.Publisher.Contains(keyword)
                ));
        }

        // ==========================================
        // Category
        // ==========================================

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == filter.CategoryId.Value);
        }

        // ==========================================
        // Author
        // ==========================================

        if (filter.AuthorId.HasValue)
        {
            query = query.Where(x =>
                x.Authors.Any(a =>
                    a.AuthorId == filter.AuthorId.Value));
        }

        // ==========================================
        // Sort
        // ==========================================

        query = filter.SortBy switch
        {
            "title" =>
                query.OrderBy(x => x.Title),

            "popular" =>
                query.OrderByDescending(x =>
                    x.Borrowrecorddetails.Count),

            "available" =>
                query.OrderByDescending(x =>
                    x.AvailableQuantity),

            _ =>
                query.OrderByDescending(x =>
                    x.CreatedAt)
        };

        // ==========================================
        // Projection
        // ==========================================

        var books = query.Select(x =>
            new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorNames = string.Join(
                    ", ",
                    x.Authors
                        .OrderBy(a => a.AuthorName)
                        .Select(a => a.AuthorName)
                ),

                CategoryName = x.Category.CategoryName,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable,

                IsFavorite = favoriteBookIds.Contains(x.BookId)
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
        int? accountId,
        int bookId)
    {
        // ==========================================
        // Lấy ResidentId
        // ==========================================

        int? residentId = null;

        if (accountId.HasValue)
        {
            residentId = await _context.Residents
                .Where(x =>
                    x.AccountId == accountId.Value)
                .Select(x =>
                    (int?)x.ResidentId)
                .FirstOrDefaultAsync();
        }

        // ==========================================
        // Chi tiết sách
        // ==========================================

        return await _context.Books

            .AsNoTracking()

            .Where(x => x.BookId == bookId)

            .Select(x => new BookDetailViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorNames = string.Join(
                    ", ",
                    x.Authors
                        .OrderBy(a => a.AuthorName)
                        .Select(a => a.AuthorName)
                ),

                CategoryName = x.Category.CategoryName,

                Publisher = x.Publisher,

                PublishYear = x.PublicationYear,

                Quantity = x.Quantity,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable,

                Description = x.BookDescription,

                IsFavorite =
                    residentId.HasValue &&
                    _context.Favoritebooks.Any(f =>
                        f.ResidentId == residentId.Value &&
                        f.BookId == x.BookId)
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
        // Lấy sách hiện tại
        // ===================================

        var currentBook = await _context.Books

            .AsNoTracking()

            .Where(x => x.BookId == bookId)

            .Select(x => new
            {
                x.CategoryId,

                AuthorIds = x.Authors
                    .Select(a => a.AuthorId)
                    .ToList()
            })

            .FirstOrDefaultAsync();

        if (currentBook == null)
        {
            return new List<BookCardViewModel>();
        }

        // ===================================
        // 1. Cùng thể loại
        // ===================================

        var relatedBooks = await _context.Books

            .AsNoTracking()

            .Where(x =>
                x.BookId != bookId &&
                x.CategoryId == currentBook.CategoryId)

            .OrderByDescending(x => x.CreatedAt)

            .Take(6)

            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                CoverImage = x.CoverImage,

                AuthorNames = string.Join(
                    ", ",
                    x.Authors
                        .OrderBy(a => a.AuthorName)
                        .Select(a => a.AuthorName)
                ),

                CategoryName = x.Category.CategoryName,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable
            })

            .ToListAsync();

        // ===================================
        // 2. Nếu chưa đủ → cùng tác giả
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
                    !existedIds.Contains(x.BookId) &&

                    x.Authors.Any(a =>
                        currentBook.AuthorIds.Contains(
                            a.AuthorId)))

                .OrderByDescending(x => x.CreatedAt)

                .Take(6 - relatedBooks.Count)

                .Select(x => new BookCardViewModel
                {
                    BookId = x.BookId,

                    Title = x.Title,

                    CoverImage = x.CoverImage,

                    AuthorNames = string.Join(
                        ", ",
                        x.Authors
                            .OrderBy(a => a.AuthorName)
                            .Select(a => a.AuthorName)
                    ),

                    CategoryName = x.Category.CategoryName,

                    AvailableQuantity = x.AvailableQuantity,

                    IsAvailable = x.IsAvailable
                })

                .ToListAsync();

            relatedBooks.AddRange(authorBooks);
        }

        // ===================================
        // 3. Nếu vẫn chưa đủ → sách mới
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

                    AuthorNames = string.Join(
                        ", ",
                        x.Authors
                            .OrderBy(a => a.AuthorName)
                            .Select(a => a.AuthorName)
                    ),

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