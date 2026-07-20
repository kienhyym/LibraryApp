using LibraryApp.Models;
using LibraryApp.ViewModels;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using LibraryApp.Results;

using System.Security.Claims;

namespace LibraryApp.Services;

public class AuthService : IAuthService
{
    private readonly LibDbContext _context;

    public AuthService(LibDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResult> LoginAsync(
        HttpContext httpContext,
        LoginViewModel model)
    {
        var taiKhoan = await GetTaiKhoanAsync(model.TenDangNhap);

        if (taiKhoan == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng."
            };
        }

        if (!taiKhoan.TrangThai)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Tài khoản đã bị khóa."
            };
        }

        if (!VerifyPassword(taiKhoan, model.MatKhau))
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng."
            };
        }

        var claims = CreateClaims(taiKhoan);

        await SignInAsync(
            httpContext,
            claims,
            model.RememberMe);

        return new LoginResult
        {
            Success = true,
            MaTaiKhoan = taiKhoan.MaTaiKhoan,
            TenDangNhap = taiKhoan.TenDangNhap,
            VaiTro = taiKhoan.VaiTro
        };
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
    private async Task<Taikhoan?> GetTaiKhoanAsync(string tenDangNhap)
    {
        return await _context.Taikhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);
    }

    private bool VerifyPassword(
    Taikhoan taiKhoan,
    string matKhauNhap)
    {
        var passwordHasher = new PasswordHasher<Taikhoan>();

        var result = passwordHasher.VerifyHashedPassword(
            taiKhoan,
            taiKhoan.MatKhau,
            matKhauNhap);

        return result != PasswordVerificationResult.Failed;
    }

    private List<Claim> CreateClaims(Taikhoan taiKhoan)
    {
        return new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            taiKhoan.MaTaiKhoan.ToString()),

        new Claim(
            ClaimTypes.Name,
            taiKhoan.TenDangNhap),

        new Claim(
            ClaimTypes.Role,
            taiKhoan.VaiTro)
    };
    }

    private async Task SignInAsync(
        HttpContext httpContext,
        List<Claim> claims,
        bool rememberMe)
    {
        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            properties);
    }
}