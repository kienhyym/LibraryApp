namespace LibraryApp.Services.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Gửi email bất kỳ.
    /// </summary>
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody);

    /// <summary>
    /// Gửi email chứa mã OTP.
    /// </summary>
    Task SendOtpAsync(
        string toEmail,
        string otpCode);
}