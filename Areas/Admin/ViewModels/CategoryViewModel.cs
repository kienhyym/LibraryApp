using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels;

public class CategoryViewModel
{
    public int CategoryId { get; set; }

    [Display(Name = "Tên thể loại")]
    [Required(ErrorMessage = "Tên thể loại không được để trống.")]
    [StringLength(100, ErrorMessage = "Tên thể loại tối đa 100 ký tự.")]
    public string CategoryName { get; set; } = string.Empty;

    [Display(Name = "Mô tả")]
    [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự.")]
    public string? CategoryDescription { get; set; }

    public DateTime CreatedAt { get; set; }
}