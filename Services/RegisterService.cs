using LibraryApp.Enums;
using LibraryApp.Helpers;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class RegisterService : IRegisterService
{
    private const string REGISTER_SESSION_KEY = "RegisterInfo";

    private readonly LibDbContext _context;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<Account> _passwordHasher;

    public RegisterService(
        LibDbContext context,
        IOtpService otpService,
        IEmailService emailService)
    {
        _context = context;
        _otpService = otpService;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<Account>();
    }

    public async Task<(bool Success, string Message)> SendOtpAsync(
        RegisterViewModel model,
        ISession session)
    {
        bool emailExists = await _context.Accounts
            .AnyAsync(x => x.Email == model.Email);

        if (emailExists)
        {
            return (false, "Email đã được sử dụng.");
        }

        // Lưu thông tin đăng ký vào Session
        SessionHelper.SetObject(
            session,
            REGISTER_SESSION_KEY,
            model);

        // Sinh OTP
        string otp = await _otpService.GenerateOtpAsync(model.Email);

        string html = $@"
            <h2>Library Management System</h2>

            <p>Xin chào,</p>

            <p>Mã OTP của bạn là:</p>

            <h1 style='color:#0d6efd'>{otp}</h1>

            <p>Mã OTP có hiệu lực trong <strong>5 phút</strong>.</p>

            <p>Vui lòng không chia sẻ mã này với bất kỳ ai.</p>";

        await _emailService.SendEmailAsync(
            model.Email,
            "Email Verification",
            html);

        return (true, "OTP đã được gửi đến email của bạn.");
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        VerifyOtpViewModel model,
        ISession session)
    {
        // Kiểm tra OTP
        bool verified = await _otpService.VerifyOtpAsync(
            model.Email,
            model.OtpCode);

        if (!verified)
        {
            return (false, "OTP không hợp lệ hoặc đã hết hạn.");
        }

        // Lấy thông tin đăng ký từ Session
        var registerInfo = SessionHelper.GetObject<RegisterViewModel>(
            session,
            REGISTER_SESSION_KEY);

        if (registerInfo == null)
        {
            return (false, "Phiên đăng ký đã hết hạn. Vui lòng đăng ký lại.");
        }

        // Kiểm tra lại email
        bool emailExists = await _context.Accounts
            .AnyAsync(x => x.Email == registerInfo.Email);

        if (emailExists)
        {
            return (false, "Email đã được sử dụng.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // ============================
            // Tạo Account
            // ============================

            var account = new Account
            {
                Email = registerInfo.Email,
                AccountRole = (int)AccountRole.Resident,
                IsActive = true,
                IsEmailVerified = true
            };

            account.PasswordHash = _passwordHasher.HashPassword(
                account,
                registerInfo.Password);

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            // ============================
            // Tạo Resident
            // ============================

            var resident = new Resident
            {
                AccountId = account.AccountId,
                FullName = registerInfo.FullName,
                DateOfBirth = registerInfo.DateOfBirth,
                Gender = registerInfo.Gender,
                PhoneNumber = registerInfo.PhoneNumber,
                ApartmentNumber = registerInfo.ApartmentNumber,
                PermanentAddress = registerInfo.PermanentAddress
            };

            _context.Residents.Add(resident);

            await _context.SaveChangesAsync();

            // ============================
            // Xóa OTP
            // ============================

            await _otpService.DeleteOtpAsync(registerInfo.Email);


            // Lưu tất cả thay đổi (bao gồm xóa OTP)
            await _context.SaveChangesAsync();

            // ============================
            // Xóa Session
            // ============================

            session.Remove(REGISTER_SESSION_KEY);

            // ============================
            // Commit
            // ============================

            await transaction.CommitAsync();

            return (true, "Đăng ký tài khoản thành công.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            return (false, "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.");
        }
    }
}