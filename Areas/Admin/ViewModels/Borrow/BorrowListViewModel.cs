using LibraryApp.Enums;

namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BorrowListViewModel
{
    public int BorrowRecordId { get; set; }

    public string ResidentName { get; set; } = string.Empty;

    public string PersonnelName { get; set; } = string.Empty;

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public BorrowRecordStatus BorrowRecordStatus { get; set; }

    public int TotalBooks { get; set; }
}