using System.ComponentModel.DataAnnotations;
using LibraryApp.Enums;

namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BorrowReturnItemViewModel
{
    public int BorrowRecordDetailId { get; set; }

    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    [Display(Name = "Tình trạng nhận sách")]
    [Required(ErrorMessage = "Vui lòng chọn tình trạng nhận sách.")]
    public ReturnStatus ReturnStatus { get; set; }
        = ReturnStatus.Received;

    [Display(Name = "Ghi chú")]
    [StringLength(
        500,
        ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? ReturnNote { get; set; }

    [Display(Name = "Tiền phạt")]
    [Range(
        typeof(decimal),
        "0",
        "10000000",
        ErrorMessage =
            "Tiền phạt phải từ 0 đến 10.000.000 VNĐ.")]
    public decimal Penalty { get; set; }
}