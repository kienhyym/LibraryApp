using LibraryApp.ViewModels.Category;

namespace LibraryApp.Services.Interfaces;

public interface ICategoriesService
{
    Task<List<CategoriesViewModel>> GetCategoriesAsync();
}