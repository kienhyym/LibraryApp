using System.Security.Cryptography;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class OtpService : IOtpService
{
    private readonly LibDbContext _context;

    private readonly IEmailService _emailService;

    public OtpService(
        LibDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    #region Send OTP

    public async Task SendOtpAsync(string email)
    {
        var otp = await GenerateOtpAsync(email);

        await _emailService.SendOtpAsync(
            email,
            otp);
    }

    #endregion

    #region Generate OTP

    public async Task<string> GenerateOtpAsync(string email)
    {
        var otp = RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();

        var existingOtp = await _context.EmailVerifications
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingOtp == null)
        {
            existingOtp = new EmailVerification
            {
                Email = email,
                OtpCode = otp,
                ExpiredAt = DateTime.Now.AddMinutes(5),
                IsVerified = false
            };

            _context.EmailVerifications.Add(existingOtp);
        }
        else
        {
            existingOtp.OtpCode = otp;
            existingOtp.ExpiredAt = DateTime.Now.AddMinutes(5);
            existingOtp.IsVerified = false;
        }

        await _context.SaveChangesAsync();

        return otp;
    }

    #endregion

    #region Verify OTP

    public async Task<bool> VerifyOtpAsync(
        string email,
        string otpCode)
    {
        var otp = await _context.EmailVerifications
            .FirstOrDefaultAsync(x => x.Email == email);

        if (otp == null)
            return false;

        if (otp.IsVerified)
            return false;

        if (otp.ExpiredAt < DateTime.Now)
            return false;

        if (otp.OtpCode != otpCode)
            return false;

        otp.IsVerified = true;

        await _context.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Check Verify

    public async Task<bool> IsEmailVerifiedAsync(string email)
    {
        return await _context.EmailVerifications
            .AnyAsync(x =>
                x.Email == email &&
                x.IsVerified);
    }

    #endregion

    #region Delete OTP

    public async Task DeleteOtpAsync(string email)
    {
        var otp = await _context.EmailVerifications
            .FirstOrDefaultAsync(x => x.Email == email);

        if (otp == null)
            return;

        _context.EmailVerifications.Remove(otp);

        await _context.SaveChangesAsync();
    }

    #endregion
}