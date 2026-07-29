using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.Areas.Admin.ViewModels;

public class BookViewModel
{
    public int BookId { get; set; }

    [Display(Name = "Tên sách")]
    [Required(ErrorMessage = "Tên sách không được để trống.")]
    [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Thể loại")]
    [Required(ErrorMessage = "Vui lòng chọn thể loại.")]
    public int CategoryId { get; set; }

    [Display(Name = "Tác giả")]
    [Required(ErrorMessage = "Vui lòng chọn tác giả.")]
    public int AuthorId { get; set; }

    [Display(Name = "Nhà xuất bản")]
    [StringLength(150, ErrorMessage = "Nhà xuất bản tối đa 150 ký tự.")]
    public string? Publisher { get; set; }

    [Display(Name = "Năm xuất bản")]
    [Range(1000, 9999, ErrorMessage = "Năm xuất bản không hợp lệ.")]
    public int? PublicationYear { get; set; }

    [Display(Name = "Số lượng")]
    [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
    public int Quantity { get; set; }

    // Chỉ hiển thị ở danh sách và chi tiết
    public int AvailableQuantity { get; set; }

    [Display(Name = "Mô tả")]
    [StringLength(4000)]
    public string? BookDescription { get; set; }

    // Đường dẫn ảnh đã lưu
    public string? CoverImage { get; set; }

    [Display(Name = "Ảnh bìa")]
    public IFormFile? CoverImageFile { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    // Hiển thị trên Index
    public string? AuthorName { get; set; }

    public string? CategoryName { get; set; }

    // Dropdown
    public List<SelectListItem> Authors { get; set; } = new();

    public List<SelectListItem> Categories { get; set; } = new();
}