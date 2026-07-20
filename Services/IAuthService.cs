using LibraryApp.ViewModels;
using LibraryApp.Results;

namespace LibraryApp.Services;

public interface IAuthService
{

    Task<LoginResult> LoginAsync(
        HttpContext httpContext,
        LoginViewModel model);

    Task LogoutAsync(HttpContext httpContext);
}