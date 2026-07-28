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

    Task<bool> AuthorNameExistsAsync(string authorName);
    Task<bool> AuthorNameExistsForUpdateAsync(string authorName, int excludeId);
    Task<bool> DeleteAsync(int authorId);
}