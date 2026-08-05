namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class ResidentLookupViewModel
{
    public int ResidentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string DisplayText =>
        $"{FullName} - {Email} - {PhoneNumber}";
}