using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class ResidentController : AdminBaseController
{
    private readonly IResidentService _residentService;

    public ResidentController(
        IResidentService residentService)
    {
        _residentService = residentService;
    }

    #region Index

    public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
    {
        const int pageSize = 10;

        var model = await _residentService.GetPagedAsync(
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
            await _residentService.GetCreateModelAsync();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        ResidentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new
            {
                success = false,
                message = "Dữ liệu không hợp lệ."
            });
        }

        try
        {
            await _residentService.SendOtpAsync(model);

            return Json(new
            {
                success = true,
                message = "Đã gửi mã OTP tới email."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Verify OTP

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(
        ResidentViewModel model)
    {
        try
        {
            await _residentService
                .VerifyOtpAndCreateAsync(model);

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action(nameof(Index))
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Resend OTP

    [HttpPost]
    public async Task<IActionResult> ResendOtp(
        string email)
    {
        try
        {
            await _residentService
                .ResendOtpAsync(email);

            return Json(new
            {
                success = true,
                message = "Đã gửi lại mã OTP."
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model =
            await _residentService
                .GetEditModelAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        ResidentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _residentService.UpdateAsync(model);

            TempData["Success"] =
                "Cập nhật cư dân thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                "",
                ex.Message);

            return View(model);
        }
    }

    #endregion

    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            await _residentService.ToggleActiveAsync(id);

            TempData["Success"] =
                "Cập nhật trạng thái tài khoản thành công.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}