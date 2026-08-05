using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BorrowCreateViewModel
{
    [Display(Name = "Cư dân")]
    [Required(ErrorMessage = "Vui lòng chọn cư dân.")]
    public int ResidentId { get; set; }

    [Display(Name = "Hạn trả")]
    [Required]
    [DataType(DataType.Date)]
    public DateOnly DueDate { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    public List<BorrowBookItemViewModel> Books { get; set; }
        = [];
}