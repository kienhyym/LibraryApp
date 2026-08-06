using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Book;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class BooksController : Controller
{
    private readonly IBooksService _booksService;

    public BooksController(
        IBooksService booksService)
    {
        _booksService = booksService;
    }

    #region Book List

    [HttpGet]
    public async Task<IActionResult> Index(
        BookFilterViewModel filter)
    {
        var model =
            await _booksService.GetBooksAsync(filter);

        return View(model);
    }

    #endregion

    #region Book Detail

    [HttpGet]
    public async Task<IActionResult> Detail(
        int id)
    {
        var model =
            await _booksService.GetBookDetailAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.RelatedBooks =
            await _booksService.GetRelatedBooksAsync(id);

        return View(model);
    }

    #endregion

    #region Category

    [HttpGet]
    public async Task<IActionResult> Category(
        int id,
        int page = 1)
    {
        var filter = new BookFilterViewModel
        {
            CategoryId = id,
            Page = page
        };

        var model =
            await _booksService.GetBooksAsync(filter);

        return View("Index", model);
    }

    #endregion

    #region Author

    [HttpGet]
    public async Task<IActionResult> Author(
        int id,
        int page = 1)
    {
        var filter = new BookFilterViewModel
        {
            AuthorId = id,
            Page = page
        };

        var model =
            await _booksService.GetBooksAsync(filter);

        return View("Index", model);
    }

    #endregion

    #region Search

    [HttpGet]
    public async Task<IActionResult> Search(
        string? keyword,
        int page = 1)
    {
        var filter = new BookFilterViewModel
        {
            Keyword = keyword,
            Page = page
        };

        var model =
            await _booksService.GetBooksAsync(filter);

        return View("Index", model);
    }

    #endregion
}