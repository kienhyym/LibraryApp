namespace LibraryApp.Services.Interfaces;

public interface IOtpService
{
    /// <summary>
    /// Generate a new OTP and save/update it in database.
    /// </summary>
    Task<string> GenerateOtpAsync(string email);

    /// <summary>
    /// Verify OTP.
    /// </summary>
    Task<bool> VerifyOtpAsync(string email, string otpCode);

    /// <summary>
    /// Check whether an email has been verified.
    /// </summary>
    Task<bool> IsEmailVerifiedAsync(string email);

    /// <summary>
    /// Remove OTP information after registration completes.
    /// </summary>
    Task DeleteOtpAsync(string email);
}