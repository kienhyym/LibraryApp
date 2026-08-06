using LibraryApp.Helpers;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.ForgotPassword;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class ForgotPasswordService : IForgotPasswordService
{
    private const string FORGOT_PASSWORD_SESSION_KEY =
        "ForgotPasswordInfo";

    private const string FORGOT_PASSWORD_VERIFIED_KEY =
        "ForgotPasswordVerified";

    private readonly LibDbContext _context;

    private readonly IOtpService _otpService;

    private readonly IEmailService _emailService;

    private readonly PasswordHasher<Account> _passwordHasher;

    public ForgotPasswordService(
        LibDbContext context,
        IOtpService otpService,
        IEmailService emailService)
    {
        _context = context;

        _otpService = otpService;

        _emailService = emailService;

        _passwordHasher =
            new PasswordHasher<Account>();
    }

    #region Send OTP

    public async Task<(bool Success, string Message)> SendOtpAsync(
        ForgotPasswordViewModel model,
        ISession session)
    {
        var account =
            await _context.Accounts
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email);

        if (account == null)
        {
            return (
                false,
                "Email không tồn tại trong hệ thống.");
        }

        if (!account.IsActive)
        {
            return (
                false,
                "Tài khoản đã bị khóa.");
        }

        SessionHelper.SetObject(
            session,
            FORGOT_PASSWORD_SESSION_KEY,
            model);

        string otp =
            await _otpService.GenerateOtpAsync(
                model.Email);

        string html = $@"
            <h2>Library Management System</h2>

            <p>Xin chào,</p>

            <p>Bạn vừa yêu cầu đặt lại mật khẩu.</p>

            <p>Mã OTP của bạn là:</p>

            <h1 style='color:#0d6efd'>
                {otp}
            </h1>

            <p>
                Mã OTP có hiệu lực trong
                <strong>5 phút</strong>.
            </p>

            <p>
                Nếu bạn không thực hiện yêu cầu này,
                vui lòng bỏ qua email.
            </p>";

        await _emailService.SendEmailAsync(
            model.Email,
            "Reset Password OTP",
            html);

        return (
            true,
            "OTP đã được gửi tới Email.");
    }

    #endregion
    #region Verify OTP

    public async Task<(bool Success, string Message)> VerifyOtpAsync(
        ForgotPasswordVerifyOtpViewModel model,
        ISession session)
    {
        // ============================
        // Kiểm tra OTP
        // ============================

        bool verified =
            await _otpService.VerifyOtpAsync(
                model.Email,
                model.OtpCode);

        if (!verified)
        {
            return (
                false,
                "OTP không hợp lệ hoặc đã hết hạn.");
        }

        // ============================
        // Lấy Session
        // ============================

        var forgotInfo =
            SessionHelper.GetObject<ForgotPasswordViewModel>(
                session,
                FORGOT_PASSWORD_SESSION_KEY);

        if (forgotInfo == null)
        {
            return (
                false,
                "Phiên xác thực đã hết hạn.");
        }

        // ============================
        // Kiểm tra Email
        // ============================

        var account =
            await _context.Accounts
                .FirstOrDefaultAsync(x =>
                    x.Email == forgotInfo.Email);

        if (account == null)
        {
            return (
                false,
                "Tài khoản không tồn tại.");
        }

        if (!account.IsActive)
        {
            return (
                false,
                "Tài khoản đã bị khóa.");
        }

        // ============================
        // Đánh dấu đã xác thực OTP
        // ============================

        session.SetString(
            FORGOT_PASSWORD_VERIFIED_KEY,
            "true");

        return (
            true,
            "Xác thực OTP thành công.");
    }

    #endregion
    #region Reset Password

    public async Task<(bool Success, string Message)> ResetPasswordAsync(
    ResetPasswordViewModel model,
    ISession session)
    {
        // ============================

        // Kiểm tra đã xác thực OTP chưa

        // ============================

        var verified =

            session.GetString(

                FORGOT_PASSWORD_VERIFIED_KEY);

        if (verified != "true")

        {

            return (

                false,

                "Vui lòng xác thực OTP trước.");

        }
        // ============================
        // Lấy thông tin từ Session
        // ============================

        var forgotInfo =
            SessionHelper.GetObject<ForgotPasswordViewModel>(
                session,
                FORGOT_PASSWORD_SESSION_KEY);

        if (forgotInfo == null)
        {
            return (
                false,
                "Phiên khôi phục mật khẩu đã hết hạn.");
        }

        // ============================
        // Tìm tài khoản
        // ============================

        var account =
     await _context.Accounts
         .FirstOrDefaultAsync(x =>
             x.Email == forgotInfo.Email);

        if (account == null)
        {
            return (
                false,
                "Tài khoản không tồn tại.");
        }

        if (!account.IsActive)
        {
            return (
                false,
                "Tài khoản đã bị khóa.");
        }

        // ============================
        // Cập nhật mật khẩu
        // ============================

        account.PasswordHash =
            _passwordHasher.HashPassword(
                account,
                model.Password);

        // ============================
        // Lưu Database
        // ============================

        await _context.SaveChangesAsync();

        // ============================
        // Xóa OTP
        // ============================

        await _otpService.DeleteOtpAsync(
    forgotInfo.Email);
        // ============================
        // Xóa Session
        // ============================

        session.Remove(
            FORGOT_PASSWORD_SESSION_KEY);

        session.Remove(
            FORGOT_PASSWORD_VERIFIED_KEY);
        return (
            true,
            "Đổi mật khẩu thành công.");
    }

    #endregion

    #region Resend OTP

    public async Task<(bool Success, string Message)> ResendOtpAsync(
        ISession session)
    {
        // ============================
        // Lấy thông tin từ Session
        // ============================

        var forgotInfo =
            SessionHelper.GetObject<ForgotPasswordViewModel>(
                session,
                FORGOT_PASSWORD_SESSION_KEY);

        if (forgotInfo == null)
        {
            return (
                false,
                "Phiên khôi phục mật khẩu đã hết hạn.");
        }

        // ============================
        // Kiểm tra Email
        // ============================

        var account =
            await _context.Accounts
                .FirstOrDefaultAsync(x =>
                    x.Email == forgotInfo.Email);

        if (account == null)
        {
            return (
                false,
                "Tài khoản không tồn tại.");
        }

        if (!account.IsActive)
        {
            return (
                false,
                "Tài khoản đã bị khóa.");
        }
        string mail = forgotInfo.Email;
        // ============================
        // Sinh OTP mới
        // ============================

        string otp =
            await _otpService.GenerateOtpAsync(mail);

        // ============================
        // Gửi Email
        // ============================

        string html = $@"
            <h2>Library Management System</h2>

            <p>Bạn vừa yêu cầu gửi lại mã OTP.</p>

            <p>Mã OTP mới của bạn là:</p>

            <h1 style='color:#0d6efd'>
                {otp}
            </h1>

            <p>
                Mã OTP có hiệu lực trong
                <strong>5 phút</strong>.
            </p>

            <p>
                Nếu bạn không thực hiện yêu cầu này,
                vui lòng bỏ qua email.
            </p>";

        await _emailService.SendEmailAsync(
            mail,
            "Reset Password OTP",
            html);

        return (
            true,
            "Đã gửi lại mã OTP.");
    }

    #endregion
}