using System.Security.Claims;
using LibraryApp.Areas.Admin.Controllers;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Information;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class InformationController : AdminBaseController
{
    private readonly IInformationService _informationService;

    public InformationController(
        IInformationService informationService)
    {
        _informationService = informationService;
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

        if (!int.TryParse(
                accountIdClaim.Value,
                out int accountId))
        {
            return Challenge();
        }

        var model =
            await _informationService
                .GetInformationAsync(accountId);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var accountIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (accountIdClaim == null)
        {
            return Challenge();
        }

        if (!int.TryParse(
                accountIdClaim.Value,
                out int accountId))
        {
            return Challenge();
        }

        var model =
            await _informationService
                .GetInformationForUpdateAsync(accountId);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        InformationUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accountIdClaim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (accountIdClaim == null)
        {
            return Challenge();
        }

        if (!int.TryParse(
                accountIdClaim.Value,
                out int accountId))
        {
            return Challenge();
        }

        var result =
            await _informationService
                .UpdateInformationAsync(
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
}