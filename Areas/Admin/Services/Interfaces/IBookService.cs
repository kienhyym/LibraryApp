using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;

namespace LibraryApp.Services.Interfaces;

public interface IBookService
{
    Task<PaginatedList<BookViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize);

    Task<BookViewModel> GetCreateModelAsync();

    Task<BookViewModel?> GetEditModelAsync(int id);

    Task<BookViewModel?> GetByIdAsync(int id);

    Task CreateAsync(BookViewModel model);

    Task UpdateAsync(BookViewModel model);

    Task DeleteAsync(int id);

    Task<bool> BookExistsByTitleAsync(string title);

    Task<bool> BookExistsByTitleForUpdateAsync(
        string title,
        int bookId);

    // NEW
    Task LoadDropdownDataAsync(BookViewModel model);
}