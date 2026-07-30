using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Resident;

public class ResidentVerifyOtpViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Mã OTP")]
    [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
    [StringLength(6, MinimumLength = 6)]
    public string OtpCode { get; set; } = string.Empty;
}