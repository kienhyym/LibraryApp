namespace LibraryApp.Services;

using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;

public interface IAuthorService
{
    Task<PaginatedList<AuthorViewModel>> GetPagedAsync(

    string? keyword,

    int page,

    int pageSize);

    Task<AuthorViewModel?> GetByIdAsync(int id);

    Task CreateAsync(AuthorViewModel model);

    Task UpdateAsync(AuthorViewModel model);

    Task<bool> AuthorExistsByNameAsync(string authorName);

    Task<bool> AuthorExistsByNameForUpdateAsync(
        string authorName,
        int authorId);
    Task<bool> DeleteAsync(int authorId);
}