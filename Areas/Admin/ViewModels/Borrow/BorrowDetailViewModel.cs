using LibraryApp.Enums;

namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BorrowDetailViewModel
{
    public int BorrowRecordId { get; set; }

    public string ResidentName { get; set; } = string.Empty;

    public string PersonnelName { get; set; } = string.Empty;

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public BorrowRecordStatus BorrowRecordStatus { get; set; }

    public string? Notes { get; set; }

    public List<BorrowBookItemViewModel> Books { get; set; }
        = [];
    public string? ReturnPersonnelName { get; set; }
}