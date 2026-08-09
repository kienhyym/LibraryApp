using LibraryApp.ViewModels.Home;

namespace LibraryApp.Services.Interfaces;

public interface IHomeService
{
    #region Home

    Task<HomeViewModel> GetHomeAsync(int? accountId);
    #endregion

    #region Search

    Task<List<BookCardViewModel>> SearchBooksAsync(
        string? keyword);

    #endregion
    Task<AboutViewModel> GetAboutAsync();
}