namespace LibraryApp.ViewModels.BorrowHistory;

public class BorrowRecordViewModel
{
    public int BorrowRecordId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int TotalBooks { get; set; }

    public List<BorrowHistoryBookViewModel> Books { get; set; }
        = new();
}