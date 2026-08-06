using LibraryApp.ViewModels.ForgotPassword;
using Microsoft.AspNetCore.Http;
namespace LibraryApp.Services.Interfaces;

public interface IForgotPasswordService
{
    #region Send OTP

    Task<(bool Success, string Message)> SendOtpAsync(
        ForgotPasswordViewModel model,
        ISession session);

    #endregion

    #region Verify OTP

    Task<(bool Success, string Message)> VerifyOtpAsync(
        ForgotPasswordVerifyOtpViewModel model,
        ISession session);

    #endregion

    #region Reset Password

    Task<(bool Success, string Message)> ResetPasswordAsync(
    ResetPasswordViewModel model,
    ISession session);

    #endregion

    #region Resend OTP

    Task<(bool Success, string Message)> ResendOtpAsync(
        ISession session);

    #endregion
}