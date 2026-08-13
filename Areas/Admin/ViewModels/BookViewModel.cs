using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.Areas.Admin.ViewModels;

public class BookViewModel
{
    public int BookId { get; set; }

    // =========================
    // Thông tin sách
    // =========================

    [Display(Name = "Tên sách")]
    [Required(ErrorMessage = "Tên sách không được để trống.")]
    [StringLength(
        200,
        ErrorMessage = "Tên sách tối đa 200 ký tự.")]
    public string Title { get; set; } = string.Empty;


    [Display(Name = "Thể loại")]
    [Required(ErrorMessage = "Vui lòng chọn thể loại.")]
    public int CategoryId { get; set; }


    // =========================
    // Tác giả
    // =========================

    [Display(Name = "Tác giả")]
    [MinLength(
        1,
        ErrorMessage = "Vui lòng chọn ít nhất một tác giả.")]
    public List<int> AuthorIds { get; set; } = new();


    // =========================
    // Xuất bản
    // =========================

    [Display(Name = "Nhà xuất bản")]
    [StringLength(
        150,
        ErrorMessage = "Nhà xuất bản tối đa 150 ký tự.")]
    public string? Publisher { get; set; }


    [Display(Name = "Năm xuất bản")]
    [Range(
        1000,
        9999,
        ErrorMessage = "Năm xuất bản không hợp lệ.")]
    public int? PublicationYear { get; set; }


    // =========================
    // Số lượng
    // =========================

    [Display(Name = "Số lượng")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
    public int Quantity { get; set; }


    // Chỉ hiển thị ở danh sách và chi tiết
    public int AvailableQuantity { get; set; }


    // =========================
    // Mô tả
    // =========================

    [Display(Name = "Mô tả")]
    [StringLength(4000)]
    public string? BookDescription { get; set; }


    // =========================
    // Ảnh
    // =========================

    // Đường dẫn ảnh đã lưu
    public string? CoverImage { get; set; }


    [Display(Name = "Ảnh bìa")]
    public IFormFile? CoverImageFile { get; set; }


    // =========================
    // Trạng thái
    // =========================

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }


    // =========================
    // Hiển thị
    // =========================

    // Danh sách tên tác giả dùng khi hiển thị
    // Ví dụ:
    // "Nguyễn Nhật Ánh, Nam Cao"
    public string? AuthorNames { get; set; }

    public string? CategoryName { get; set; }


    // =========================
    // Dropdown
    // =========================

    public List<SelectListItem> Authors { get; set; }
        = new();

    public List<SelectListItem> Categories { get; set; }
        = new();
}