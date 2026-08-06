using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class AuthorsController : Controller
{
    private readonly IAuthorsService _authorsService;

    public AuthorsController(
        IAuthorsService authorsService)
    {
        _authorsService = authorsService;
    }

    public async Task<IActionResult> Index()
    {
        var model =
            await _authorsService.GetAuthorsAsync();

        return View(model);
    }
}