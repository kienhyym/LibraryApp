namespace LibraryApp.ViewModels.BorrowHistory;

public class BorrowHistoryFilter
{
    public string Status { get; set; } = "all";

    public int Page { get; set; } = 1;
}