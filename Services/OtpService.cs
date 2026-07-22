using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class OtpService : IOtpService
{
    private readonly LibDbContext _context;

    public OtpService(LibDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateOtpAsync(string email)
    {
        // Generate random 6-digit OTP
        var random = new Random();
        var otp = random.Next(100000, 999999).ToString();

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

    public async Task<bool> VerifyOtpAsync(string email, string otpCode)
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

    public async Task<bool> IsEmailVerifiedAsync(string email)
    {
        return await _context.EmailVerifications
            .AnyAsync(x => x.Email == email && x.IsVerified);
    }

    public async Task DeleteOtpAsync(string email)
{
    var otp = await _context.EmailVerifications
        .FirstOrDefaultAsync(x => x.Email == email);

    if (otp != null)
    {
        _context.EmailVerifications.Remove(otp);
    }
}
}