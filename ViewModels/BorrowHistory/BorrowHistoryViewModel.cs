using LibraryApp.Common;

namespace LibraryApp.ViewModels.BorrowHistory;

public class BorrowHistoryViewModel
{
    public BorrowHistoryFilter Filter { get; set; }
        = new();

    public PaginatedList<BorrowRecordViewModel> BorrowRecords
        { get; set; } = default!;
}