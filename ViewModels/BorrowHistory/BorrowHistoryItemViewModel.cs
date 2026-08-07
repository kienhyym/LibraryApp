namespace LibraryApp.ViewModels.BorrowHistory;

public class BorrowHistoryItemViewModel
{
    public int BorrowRecordDetailId { get; set; }

    public int BorrowRecordId { get; set; }

    public int BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string? CoverImage { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = string.Empty;
}