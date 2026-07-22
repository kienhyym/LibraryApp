using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

public class RegisterController : Controller
{
    private readonly IRegisterService _registerService;

    public RegisterController(IRegisterService registerService)
    {
        _registerService = registerService;
    }

    /// <summary>
    /// Hiển thị trang đăng ký
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Gửi OTP
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _registerService.SendOtpAsync(
            model,
            HttpContext.Session);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        TempData["Success"] = result.Message;

        return RedirectToAction(nameof(VerifyOtp), new
        {
            email = model.Email
        });
    }

    /// <summary>
    /// Hiển thị trang nhập OTP
    /// </summary>
    [HttpGet]
    public IActionResult VerifyOtp(string email)
    {
        return View(new VerifyOtpViewModel
        {
            Email = email
        });
    }

    /// <summary>
    /// Xác thực OTP và tạo tài khoản
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(
        VerifyOtpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _registerService.RegisterAsync(
            model,
            HttpContext.Session);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        TempData["Success"] = result.Message;

        return RedirectToAction(
            "Index",
            "Login");
    }
}