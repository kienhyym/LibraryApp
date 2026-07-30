using LibraryApp.Areas.Admin.ViewModels.Personnel;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class PersonnelController : AdminBaseController
{
    private readonly IPersonnelService _personnelService;

    public PersonnelController(
        IPersonnelService personnelService)
    {
        _personnelService = personnelService;
    }

    #region Index

    public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
    {
        const int pageSize = 10;

        var model = await _personnelService.GetPagedAsync(
            keyword,
            page,
            pageSize);

        ViewBag.Keyword = keyword;

        return View(model);
    }

    #endregion

    #region Create

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model =
            await _personnelService.GetCreateModelAsync();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PersonnelCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _personnelService.CreateAsync(model);

            TempData["Success"] =
                "Thêm nhân viên thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Email"))
            {
                ModelState.AddModelError(nameof(model.Email), ex.Message);
            }
            else if (ex.Message.Contains("Số điện thoại"))
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), ex.Message);
            }
            else
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model =
            await _personnelService.GetEditModelAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    PersonnelEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _personnelService.UpdateAsync(model);

            TempData["Success"] =
                "Cập nhật nhân viên thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                nameof(model.Email),
                ex.Message);

            return View(model);
        }
    }

    #endregion

    #region Toggle Active

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            await _personnelService
                .ToggleActiveAsync(id);

            TempData["Success"] =
                "Cập nhật trạng thái thành công.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}