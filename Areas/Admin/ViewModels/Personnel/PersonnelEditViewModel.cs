using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Personnel;

public class PersonnelEditViewModel
{
    public int PersonnelId { get; set; }

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
    public int? Gender { get; set; }

    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Địa chỉ")]
    [StringLength(255)]
    public string? PersonnelAddress { get; set; }

    [Display(Name = "Trạng thái")]
    public bool IsActive { get; set; }
}