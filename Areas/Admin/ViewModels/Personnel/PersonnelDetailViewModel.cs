using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Personnel;

public class PersonnelDetailViewModel
{
    public int PersonnelId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PersonnelAddress { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}