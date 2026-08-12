using System.Security.Claims;
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

    /// <summary>
    /// Lấy ID tài khoản hiện tại.
    /// Admin, Personnel và Resident đều có thể truy cập Client.
    /// Nếu chưa có claim thì trả về null.
    /// </summary>
    private int? GetAccountId()
    {
        var claim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (int.TryParse(
                claim,
                out var accountId))
        {
            return accountId;
        }

        return null;
    }


    #region Book List

    [HttpGet]
    public async Task<IActionResult> Index(
        BookFilterViewModel filter)
    {
        var accountId = GetAccountId();

        var model =
            await _booksService.GetBooksAsync(
                accountId,
                filter);

        return View(model);
    }

    #endregion


    #region Book Detail

    [HttpGet]
    public async Task<IActionResult> Detail(
        int id)
    {
        var accountId = GetAccountId();

        var model =
            await _booksService.GetBookDetailAsync(
                accountId,
                id);

        if (model == null)
        {
            return NotFound();
        }

        ViewBag.RelatedBooks =
            await _booksService
                .GetRelatedBooksAsync(id);

        return View(model);
    }

    #endregion


    #region Category

    [HttpGet]
    public async Task<IActionResult> Category(
        int id,
        int page = 1)
    {
        var filter =
            new BookFilterViewModel
            {
                CategoryId = id,
                Page = page
            };

        var accountId = GetAccountId();

        var model =
            await _booksService.GetBooksAsync(
                accountId,
                filter);

        return View("Index", model);
    }

    #endregion


    #region Author

    [HttpGet]
    public async Task<IActionResult> Author(
        int id,
        int page = 1)
    {
        var filter =
            new BookFilterViewModel
            {
                AuthorId = id,
                Page = page
            };

        var accountId = GetAccountId();

        var model =
            await _booksService.GetBooksAsync(
                accountId,
                filter);

        return View("Index", model);
    }

    #endregion


    #region Search

    [HttpGet]
    public async Task<IActionResult> Search(
        string? keyword,
        int page = 1)
    {
        var filter =
            new BookFilterViewModel
            {
                Keyword = keyword,
                Page = page
            };

        var accountId = GetAccountId();

        var model =
            await _booksService.GetBooksAsync(
                accountId,
                filter);

        return View("Index", model);
    }

    #endregion
}