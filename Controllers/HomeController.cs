using System.Security.Claims;
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

        int? accountId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            accountId = int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        var model = await _homeService.GetHomeAsync(accountId);

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