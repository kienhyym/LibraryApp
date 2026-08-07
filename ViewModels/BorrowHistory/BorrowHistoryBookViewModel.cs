using LibraryApp.Enums;

namespace LibraryApp.ViewModels.BorrowHistory;

public class BorrowHistoryBookViewModel
{
    public int BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string? CoverImage { get; set; }

    public DateTime? ReturnDate { get; set; }

    public ReturnStatus? ReturnStatus { get; set; }
}