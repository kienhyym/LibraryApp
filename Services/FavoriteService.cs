using LibraryApp.Common;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Favorite;
using LibraryApp.ViewModels.Home;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class FavoriteService : IFavoriteService
{
    private readonly LibDbContext _context;

    private const int PageSize = 10;

    public FavoriteService(LibDbContext context)
    {
        _context = context;
    }

    private async Task<int?> GetResidentIdAsync(int accountId)
    {
        return await _context.Residents
            .Where(x => x.AccountId == accountId)
            .Select(x => (int?)x.ResidentId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> AddAsync(
        int accountId,
        int bookId)
    {
        var residentId = await GetResidentIdAsync(accountId);

        if (residentId == null)
            return false;

        var existed = await _context.Favoritebooks
            .AnyAsync(x =>
                x.ResidentId == residentId &&
                x.BookId == bookId);

        if (existed)
            return false;

        _context.Favoritebooks.Add(new Favoritebook
        {
            ResidentId = residentId.Value,
            BookId = bookId
        });

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveAsync(
        int accountId,
        int bookId)
    {
        var residentId = await GetResidentIdAsync(accountId);

        if (residentId == null)
            return false;

        var favorite = await _context.Favoritebooks
            .FirstOrDefaultAsync(x =>
                x.ResidentId == residentId &&
                x.BookId == bookId);

        if (favorite == null)
            return false;

        _context.Favoritebooks.Remove(favorite);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsFavoriteAsync(
        int accountId,
        int bookId)
    {
        var residentId = await GetResidentIdAsync(accountId);

        if (residentId == null)
            return false;

        return await _context.Favoritebooks
            .AsNoTracking()
            .AnyAsync(x =>
                x.ResidentId == residentId &&
                x.BookId == bookId);
    }

    public async Task<FavoriteViewModel> GetFavoritesAsync(
        int accountId,
        int page = 1)
    {
        var residentId = await GetResidentIdAsync(accountId);

        if (residentId == null)
        {
            return new FavoriteViewModel
            {
                Books = await PaginatedList<BookCardViewModel>
                    .CreateAsync(
                        Enumerable.Empty<BookCardViewModel>().AsQueryable(),
                        1,
                        PageSize)
            };
        }

        var query = _context.Favoritebooks
            .AsNoTracking()
            .Where(x => x.ResidentId == residentId)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new BookCardViewModel
            {
                BookId = x.BookId,

                Title = x.Book.Title,

                CoverImage = x.Book.CoverImage,

                AuthorName = x.Book.Author.AuthorName,

                CategoryName = x.Book.Category.CategoryName,

                AvailableQuantity = x.Book.AvailableQuantity,

                IsAvailable = x.Book.IsAvailable,

                IsFavorite = true
            });

        return new FavoriteViewModel
        {
            Books = await PaginatedList<BookCardViewModel>
                .CreateAsync(
                    query,
                    page,
                    PageSize)
        };
    }
}