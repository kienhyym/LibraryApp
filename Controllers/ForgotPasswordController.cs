using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.ForgotPassword;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class ForgotPasswordController : Controller
{
    private readonly IForgotPasswordService _forgotPasswordService;

    public ForgotPasswordController(
        IForgotPasswordService forgotPasswordService)
    {
        _forgotPasswordService = forgotPasswordService;
    }

    #region Index

    [HttpGet]
    public IActionResult Index()
    {
        return View(
            new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _forgotPasswordService.SendOtpAsync(
                model,
                HttpContext.Session);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        return RedirectToAction(
            nameof(VerifyOtp),
            new
            {
                email = model.Email
            });
    }

    #endregion

    #region Verify OTP

    [HttpGet]
    public IActionResult VerifyOtp(
        string email)
    {
        var model =
            new ForgotPasswordVerifyOtpViewModel
            {
                Email = email
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(
        ForgotPasswordVerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _forgotPasswordService.VerifyOtpAsync(
                model,
                HttpContext.Session);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        return RedirectToAction(
            nameof(ResetPassword),
            new
            {
                email = model.Email
            });
    }

    #endregion

    #region Reset Password

    [HttpGet]
    public IActionResult ResetPassword(
        string email)
    {
        var model =
            new ResetPasswordViewModel
            {
                Email = email
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result =
            await _forgotPasswordService.ResetPasswordAsync(
                model,
                HttpContext.Session);

        if (!result.Success)
        {
            ModelState.AddModelError(
                string.Empty,
                result.Message);

            return View(model);
        }

        TempData["Success"] =
            "Đổi mật khẩu thành công. Vui lòng đăng nhập.";

        return RedirectToAction(
            "Index",
            "Login");
    }

    #endregion

    #region Resend OTP

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequest request)
    {
        var result =
            await _forgotPasswordService.ResendOtpAsync(
                HttpContext.Session);

        return Json(new
        {
            success = result.Success,
            message = result.Message
        });
    }

    #endregion
}