namespace LibraryApp.Areas.Admin.ViewModels.Resident;

public class ResidentDetailViewModel
{
    public int ResidentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? PermanentAddress { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}