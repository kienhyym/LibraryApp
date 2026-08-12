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
    "10000000",
    ErrorMessage = "Tiền phạt phải từ 0 đến 10.000.000 VNĐ."
)]
    public decimal Penalty { get; set; }
}