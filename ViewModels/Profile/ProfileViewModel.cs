using LibraryApp.Enums;

namespace LibraryApp.ViewModels.Profile;

public class ProfileViewModel
{
    public int ResidentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? PermanentAddress { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    // Dashboard

    public int TotalBorrowedBooks { get; set; }

    public int BorrowingBooks { get; set; }

    public int OverdueBooks { get; set; }
}