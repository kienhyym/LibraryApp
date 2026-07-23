using System.ComponentModel.DataAnnotations;

namespace LibraryApp.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password),
        ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth { get; set; } = new DateOnly(2000, 1, 1);

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    public int Gender { get; set; }

    [Phone(ErrorMessage = "Invalid phone number.")]
    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số căn hộ")]
    [StringLength(20)]
    public string? ApartmentNumber { get; set; }

    [StringLength(255)]
    public string? PermanentAddress { get; set; }
}