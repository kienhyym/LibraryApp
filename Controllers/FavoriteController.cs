using System.Security.Claims;

using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

[Authorize]
public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(
        IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = await _favoriteService
            .GetFavoritesAsync(accountId, page);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(
        int bookId,
        string? returnUrl)
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isFavorite = await _favoriteService
            .IsFavoriteAsync(accountId, bookId);

        if (isFavorite)
        {
            await _favoriteService.RemoveAsync(
                accountId,
                bookId);

            TempData["Success"] = "Đã xóa khỏi danh sách yêu thích.";
        }
        else
        {
            await _favoriteService.AddAsync(
                accountId,
                bookId);

            TempData["Success"] = "Đã thêm vào danh sách yêu thích.";
        }

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAjax(int bookId)
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isFavorite =
            await _favoriteService.IsFavoriteAsync(
                accountId,
                bookId);

        if (isFavorite)
        {
            await _favoriteService.RemoveAsync(
                accountId,
                bookId);

            return Json(new
            {
                success = true,

                isFavorite = false,

                message = "Đã xóa khỏi danh sách yêu thích.",

                type = "remove"
            });
        }

        await _favoriteService.AddAsync(
            accountId,
            bookId);

        return Json(new
        {
            success = true,

            isFavorite = true,

            message = "Đã thêm vào danh sách yêu thích.",

            type = "success"
        });
    }
}