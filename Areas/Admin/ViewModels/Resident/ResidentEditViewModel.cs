using System.ComponentModel.DataAnnotations;
using LibraryApp.Enums;

namespace LibraryApp.Areas.Admin.ViewModels.Resident;

public class ResidentEditViewModel
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

    [Display(Name = "Ngày sinh")]
    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public Gender? Gender { get; set; }

    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Số căn hộ")]
    [StringLength(20)]
    public string? ApartmentNumber { get; set; }

    [Display(Name = "Địa chỉ")]
    [StringLength(255)]
    public string? PermanentAddress { get; set; }

    [Display(Name = "Trạng thái")]
    public bool IsActive { get; set; }
}