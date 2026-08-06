using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Home;

using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class HomeService : IHomeService
{
    private readonly LibDbContext _context;

    public HomeService(
        LibDbContext context)
    {
        _context = context;

    }

    #region Home

    public async Task<HomeViewModel> GetHomeAsync()
    {
        var model = new HomeViewModel();


        // ==========================
        // Sách mới
        // ==========================

        model.NewBooks = await _context.Books

            .AsNoTracking()

            .Where(x =>
    x.IsAvailable &&
    x.AvailableQuantity > 0)

            .OrderByDescending(x => x.CreatedAt)

            .Take(8)

            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                CoverImage = x.CoverImage,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable
            })

            .ToListAsync();

        // ==========================
        // Sách nổi bật
        // ==========================

        var popularBookIds =
    await _context.Borrowrecorddetails

        .AsNoTracking()

        .GroupBy(x => x.BookId)

        .OrderByDescending(x => x.Count())

        .Take(8)

        .Select(x => x.Key)

        .ToListAsync();

        model.PopularBooks =
            await _context.Books

                .AsNoTracking()

                .Where(x =>
                    popularBookIds.Contains(x.BookId))

                .Select(x => new BookCardViewModel
                {
                    BookId = x.BookId,

                    Title = x.Title,

                    AuthorName = x.Author.AuthorName,

                    CategoryName = x.Category.CategoryName,

                    CoverImage = x.CoverImage,

                    AvailableQuantity = x.AvailableQuantity,

                    IsAvailable = x.IsAvailable
                })

                .ToListAsync();

        // ==========================
        // Thể loại
        // ==========================

        model.Categories = await _context.Categories

            .AsNoTracking()

            .OrderBy(x => x.CategoryName)

            .Select(x => new CategoryCardViewModel
            {
                CategoryId = x.CategoryId,

                CategoryName = x.CategoryName,

                TotalBooks = x.Books.Count()
            })

            .ToListAsync();

        // ==========================
        // Thống kê
        // ==========================

        model.TotalBooks =
            await _context.Books.CountAsync();

        model.TotalAuthors =
            await _context.Authors.CountAsync();

        model.TotalCategories =
            await _context.Categories.CountAsync();
        model.TotalResidents =
    await _context.Residents.CountAsync();

        model.TotalBorrowRecords =
            await _context.Borrowrecords.CountAsync();
        return model;
    }

    #endregion
    #region Search

    public async Task<List<BookCardViewModel>> SearchBooksAsync(
        string? keyword)
    {
        keyword = keyword?.Trim();

        var query = _context.Books

            .AsNoTracking()

            .Where(x => x.IsAvailable);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>

                x.Title.Contains(keyword)

                ||

                x.Author.AuthorName.Contains(keyword)

                ||

                x.Category.CategoryName.Contains(keyword));
        }

        return await query

            .OrderBy(x => x.Title)

            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                AuthorName = x.Author.AuthorName,

                CategoryName = x.Category.CategoryName,

                CoverImage = x.CoverImage,

                AvailableQuantity = x.AvailableQuantity,

                IsAvailable = x.IsAvailable
            })

            .ToListAsync();
    }

    #endregion

    public async Task<AboutViewModel> GetAboutAsync()
    {
        return new AboutViewModel
        {
            TotalBooks = await _context.Books.CountAsync(),

            TotalCategories = await _context.Categories.CountAsync(),

            TotalAuthors = await _context.Authors.CountAsync(),

            TotalResidents = await _context.Residents.CountAsync()
        };
    }
}