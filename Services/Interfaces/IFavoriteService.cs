using LibraryApp.ViewModels.Favorite;

namespace LibraryApp.Services.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteViewModel> GetFavoritesAsync(
        int accountId,
        int page = 1);

    Task<bool> AddAsync(
        int accountId,
        int bookId);

    Task<bool> RemoveAsync(
        int accountId,
        int bookId);

    Task<bool> IsFavoriteAsync(
        int accountId,
        int bookId);
}