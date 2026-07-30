using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Resident;

public class ResidentListViewModel
{
    public int ResidentId { get; set; }

    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Số căn hộ")]
    public string? ApartmentNumber { get; set; }

    [Display(Name = "Trạng thái")]
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}