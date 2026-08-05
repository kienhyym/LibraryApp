using LibraryApp.ViewModels;
using Microsoft.AspNetCore.Http;

namespace LibraryApp.Services.Interfaces;

public interface IRegisterService
{
    Task<(bool Success, string Message)> SendOtpAsync(
        RegisterViewModel model,
        ISession session);

    Task<(bool Success, string Message)> RegisterAsync(
        VerifyOtpViewModel model,
        ISession session);

        Task<(bool Success, string Message)> ResendOtpAsync(
    string email,
    ISession session);
}