using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels;

public class AuthorViewModel
{
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Tên tác giả không được để trống.")]
    [StringLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Nationality { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}