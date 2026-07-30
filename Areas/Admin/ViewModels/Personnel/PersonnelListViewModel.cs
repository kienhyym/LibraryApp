using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Personnel;

public class PersonnelListViewModel
{
    public int PersonnelId { get; set; }

    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Trạng thái")]
    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}