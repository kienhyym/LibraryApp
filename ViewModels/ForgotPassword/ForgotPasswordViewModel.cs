using System.ComponentModel.DataAnnotations;

namespace LibraryApp.ViewModels.ForgotPassword;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập Email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}