using LibraryApp.Areas.Admin.ViewModels.Resident;
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
     ResidentCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _residentService.SendOtpAsync(
                model);

            TempData["Resident"] = System.Text.Json.JsonSerializer.Serialize(model);

            return RedirectToAction(
                nameof(VerifyOtp),
                new { email = model.Email });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Số điện thoại"))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    ex.Message);
            }
            else
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    ex.Message);
            }

            return View(model);
        }
    }

    #endregion


    #region Verify OTP

    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        return View(
            new ResidentVerifyOtpViewModel
            {
                Email = email
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(
        ResidentVerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var resident =
                System.Text.Json.JsonSerializer.Deserialize<ResidentCreateViewModel>(
                    TempData["Resident"]!.ToString()!);

            if (resident == null)
            {
                TempData["Error"] =
                    "Phiên đăng ký đã hết hạn.";

                return RedirectToAction(nameof(Create));
            }

            await _residentService
                .VerifyOtpAndCreateAsync(
                    resident,
                    model);

            TempData["Success"] =
                "Thêm cư dân thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                nameof(model.OtpCode),
                ex.Message);

            TempData.Keep("Resident");

            return View(model);
        }
    }

    #endregion


    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model =
            await _residentService.GetEditModelAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    ResidentEditViewModel model)
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
            if (ex.Message.Contains("Số điện thoại"))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    ex.Message);
            }
            else
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    ex.Message);
            }

            return View(model);
        }
    }

    #endregion
    #region Toggle Active

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(
        int id)
    {
        try
        {
            await _residentService.ToggleActiveAsync(id);

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendOtp(
     ResidentVerifyOtpViewModel model)
    {
        try
        {
            await _residentService.ResendOtpAsync(
                model.Email);

            TempData["Success"] =
                "Đã gửi lại mã OTP.";

            return RedirectToAction(
                nameof(VerifyOtp),
                new { email = model.Email });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                nameof(VerifyOtp),
                new { email = model.Email });
        }
    }
}