using LibraryApp.Models;
using LibraryApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LibraryApp.Services;

namespace LibraryApp.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;

    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(HttpContext, model);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.ErrorMessage!);
            return View(model);
        }

        if (result.Role == "Admin" ||
      result.Role == "Personnel")
        {
            return RedirectToAction(
                "Index",
                "Dashboard",
                new { area = "Admin" });
        }

        return RedirectToAction(
            "Index",
            "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync(HttpContext);

        return RedirectToAction(nameof(Index));
    }
    
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}