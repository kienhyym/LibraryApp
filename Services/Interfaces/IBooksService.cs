using LibraryApp.ViewModels.Book;
using LibraryApp.ViewModels.Home;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.Services.Interfaces;

public interface IBooksService
{
    #region Book List

    Task<BookListViewModel> GetBooksAsync(
        int? accountId,
        BookFilterViewModel filter);

    #endregion

    #region Book Detail

    Task<BookDetailViewModel?> GetBookDetailAsync(
        int? accountId,
        int bookId);

    #endregion

    #region Dropdown

    Task<List<SelectListItem>> GetCategoriesAsync();

    Task<List<SelectListItem>> GetAuthorsAsync();

    #endregion

    #region Related Books

    Task<List<BookCardViewModel>> GetRelatedBooksAsync(
        int bookId);

    #endregion
}