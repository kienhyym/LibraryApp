using System.ComponentModel.DataAnnotations;

namespace LibraryApp.ViewModels.ForgotPassword;

public class ForgotPasswordVerifyOtpViewModel
{
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập OTP.")]
    [StringLength(6)]
    public string OtpCode { get; set; } = string.Empty;
}