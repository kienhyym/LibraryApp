using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class RegisterController : Controller
{
    private readonly IRegisterService _registerService;

    public RegisterController(
        IRegisterService registerService)
    {
        _registerService = registerService;
    }

    #region Register

    [HttpGet]
    public IActionResult Index()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result =
            await _registerService.SendOtpAsync(
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
        return View(new VerifyOtpViewModel
        {
            Email = email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(
        VerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result =
            await _registerService.RegisterAsync(
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
            "Đăng ký tài khoản thành công.";

        return RedirectToAction(
            "Index",
            "Login");
    }

    #endregion

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendOtp(
        [FromBody] ResendOtpRequest request)
    {
        try
        {
            var result = await _registerService.ResendOtpAsync(

            request.Email,

            HttpContext.Session);

            return Json(new

            {

                success = result.Success,

                message = result.Message

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
}