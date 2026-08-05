
using System.ComponentModel.DataAnnotations;

namespace LibraryApp.ViewModels;
public class ResendOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
}