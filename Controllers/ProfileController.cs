using System.Security.Claims;

using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LibraryApp.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(
        IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var accountIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (accountIdClaim == null)
        {
            return Challenge();
        }

        int accountId = int.Parse(accountIdClaim.Value);

        var model =
            await _profileService.GetProfileAsync(accountId);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model =
            await _profileService.GetProfileForUpdateAsync(accountId);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ProfileUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result =
            await _profileService.UpdateProfileAsync(
                accountId,
                model);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        TempData["Success"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword(
    string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
    ChangePasswordViewModel model,
    string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accountIdClaim =
    User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (accountIdClaim == null)
        {
            return Challenge();
        }

        if (!int.TryParse(
                accountIdClaim,
                out int accountId))
        {
            return Challenge();
        }

        var result =
            await _profileService.ChangePasswordAsync(
                accountId,
                model);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        TempData["Success"] = result.Message;

        // Đổi mật khẩu thành công → quay về nơi đã bắt đầu

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(ChangePassword));
    }
}