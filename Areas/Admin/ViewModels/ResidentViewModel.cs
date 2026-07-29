using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels;

public class ResidentViewModel
{
    public int ResidentId { get; set; }

    public int AccountId { get; set; }

    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "Mật khẩu từ 6 ký tự trở lên.")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Xác nhận mật khẩu")]
    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
    ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public int? Gender { get; set; }

    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Số căn hộ")]
    [StringLength(20)]
    public string? ApartmentNumber { get; set; }

    [Display(Name = "Địa chỉ")]
    [StringLength(255)]
    public string? PermanentAddress { get; set; }

    // OTP dùng trong Modal
    public string? OtpCode { get; set; }

    // Hiển thị
    public bool IsEmailVerified { get; set; }

    [Display(Name = "Trạng thái")]
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

}