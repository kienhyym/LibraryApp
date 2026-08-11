using LibraryApp.Enums;

public class BorrowBookItemViewModel
{
    public int BorrowRecordDetailId { get; set; }

    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int AvailableQuantity { get; set; }

    public ReturnStatus? ReturnStatus { get; set; }

    public string? ReturnNote { get; set; }

    public DateTime? ReturnDate { get; set; }
    public decimal Penalty { get; set; }
}