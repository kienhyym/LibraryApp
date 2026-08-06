using System.ComponentModel.DataAnnotations;
using LibraryApp.Enums;

namespace LibraryApp.ViewModels.Profile;

public class ProfileUpdateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    public Gender? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số căn hộ.")]
    [StringLength(20)]
    public string? ApartmentNumber { get; set; }

    [StringLength(255)]
    public string? PermanentAddress { get; set; }

    // Chỉ để hiển thị

    public string Email { get; set; } = string.Empty;
}