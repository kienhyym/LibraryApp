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
        var account = await GetAccountAsync(model.email);

        if (account == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Email hoặc mật khẩu không đúng."
            };
        }

        if (!account.IsActive)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Tài khoản đã bị khóa."
            };
        }

        if (!VerifyPassword(account, model.MatKhau))
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Email hoặc mật khẩu không đúng."
            };
        }

        var claims = CreateClaims(account);

        await SignInAsync(
            httpContext,
            claims,
            model.RememberMe);

        return new LoginResult
        {
            Success = true,
            AccountId = account.AccountId,
            Email = account.Email,
            Role = account.AccountRole.ToString()
        };
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
    private async Task<Account?> GetAccountAsync(string email)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    private bool VerifyPassword(
    Account account,
    string password)
{
    var passwordHasher = new PasswordHasher<Account>();

    var result = passwordHasher.VerifyHashedPassword(
        account,
        account.PasswordHash,
        password);

    return result != PasswordVerificationResult.Failed;
}

    private List<Claim> CreateClaims(Account account)
    {
        return new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            account.AccountId.ToString()),

        new Claim(
            ClaimTypes.Name,
            account.Email),

        new Claim(
            ClaimTypes.Role,
            account.AccountRole.ToString())
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