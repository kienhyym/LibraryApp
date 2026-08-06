using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;

    public HomeController(
        IHomeService homeService)
    {
        _homeService = homeService;
    }

    #region Home

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model =
            await _homeService.GetHomeAsync();

        return View(model);
    }

    #endregion

    #region Search

    [HttpGet]
    public async Task<IActionResult> Search(
        string? keyword)
    {
        var books =
            await _homeService.SearchBooksAsync(
                keyword);

        ViewBag.Keyword = keyword;

        return View(books);
    }

    #endregion
    #region About
    public async Task<IActionResult> About()
    {
        var model =
            await _homeService.GetAboutAsync();

        return View(model);
    }
    #endregion
}