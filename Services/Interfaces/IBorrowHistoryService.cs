using LibraryApp.ViewModels.BorrowHistory;

namespace LibraryApp.Services.Interfaces;

public interface IBorrowHistoryService
{
    Task<BorrowHistoryViewModel> GetBorrowHistoryAsync(
        int accountId,
        BorrowHistoryFilter filter);

    Task<BorrowRecordViewModel?> GetBorrowRecordAsync(
        int accountId,
        int borrowRecordId);
}