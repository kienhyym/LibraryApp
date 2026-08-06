using LibraryApp.ViewModels.Author;

namespace LibraryApp.Services.Interfaces;

public interface IAuthorsService
{
    Task<List<AuthorsViewModel>> GetAuthorsAsync();
}