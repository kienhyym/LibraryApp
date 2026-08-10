using LibraryApp.Enums;

namespace LibraryApp.ViewModels.Information;

public class InformationViewModel
{
    public int PersonnelId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PersonnelAddress { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }
}