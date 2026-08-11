using System.ComponentModel.DataAnnotations;
using LibraryApp.Enums;

namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BorrowReturnItemViewModel
{
    public int BorrowRecordDetailId { get; set; }

    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    [Display(Name = "Kết quả trả")]
    public ReturnStatus ReturnStatus { get; set; }
        = ReturnStatus.Returned;

    [Display(Name = "Ghi chú")]
    public string? ReturnNote { get; set; }
    [Display(Name = "Tiền phạt")]
    [Range(
    typeof(decimal),
    "0",
    "9999999999999999.99",
    ErrorMessage = "Tiền phạt phải lớn hơn hoặc bằng 0.")]
    public decimal Penalty { get; set; }
}