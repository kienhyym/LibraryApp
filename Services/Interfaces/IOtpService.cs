namespace LibraryApp.Services.Interfaces;

public interface IOtpService
{
    /// <summary>
    /// Generate OTP, lưu vào database và gửi email.
    /// </summary>
    Task SendOtpAsync(string email);

    /// <summary>
    /// Generate OTP và lưu vào database.
    /// </summary>
    Task<string> GenerateOtpAsync(string email);

    /// <summary>
    /// Xác thực OTP.
    /// </summary>
    Task<bool> VerifyOtpAsync(
        string email,
        string otpCode);

    /// <summary>
    /// Kiểm tra email đã xác thực OTP hay chưa.
    /// </summary>
    Task<bool> IsEmailVerifiedAsync(string email);

    /// <summary>
    /// Xóa OTP sau khi hoàn thành.
    /// </summary>
    Task DeleteOtpAsync(string email);
}