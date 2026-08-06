using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(
        ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    public async Task<IActionResult> Index()
    {
        var model =
            await _categoriesService.GetCategoriesAsync();

        return View(model);
    }
}