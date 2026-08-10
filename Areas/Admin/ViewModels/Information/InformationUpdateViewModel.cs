using System.ComponentModel.DataAnnotations;
using LibraryApp.Enums;

namespace LibraryApp.ViewModels.Information;

public class InformationUpdateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    public Gender? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    public string? PhoneNumber { get; set; }

    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
    public string? PersonnelAddress { get; set; }

    // Chỉ hiển thị, không cho sửa
    public string Email { get; set; } = string.Empty;

}