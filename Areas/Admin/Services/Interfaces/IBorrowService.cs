using LibraryApp.Areas.Admin.ViewModels.Borrow;
using LibraryApp.Common;
using LibraryApp.Enums;

namespace LibraryApp.Services.Interfaces;

public interface IBorrowService
{
    #region Query

    Task<PaginatedList<BorrowListViewModel>> GetPagedAsync(
        string? keyword,
        BorrowRecordStatus? status,
        int page,
        int pageSize);

    Task<BorrowCreateViewModel> GetCreateModelAsync();

    Task<BorrowDetailViewModel?> GetDetailAsync(
        int borrowRecordId);

    #endregion

    #region Lookup

    /// <summary>
    /// Tìm cư dân theo họ tên, email hoặc số điện thoại.
    /// </summary>
    Task<List<ResidentLookupViewModel>> SearchResidentsAsync(
        string? keyword);

    /// <summary>
    /// Tìm sách theo tên sách, tác giả hoặc thể loại.
    /// </summary>
    Task<List<BookLookupViewModel>> SearchBooksAsync(
        string? keyword);

    #endregion

    #region Borrow

    /// <summary>
    /// Tạo phiếu mượn.
    /// </summary>
    Task CreateAsync(
        BorrowCreateViewModel model,
        int personnelId);

    #endregion

    #region Return

    /// <summary>
    /// Trả sách.
    /// </summary>
    Task ReturnBooksAsync(
    int borrowRecordId,
    List<BorrowReturnItemViewModel> books,
    int returnPersonnelId);

    #endregion

    #region Validation

    /// <summary>
    /// Kiểm tra cư dân còn được phép mượn sách.
    /// </summary>
    Task<bool> CanBorrowAsync(
        int residentId);

    /// <summary>
    /// Kiểm tra sách còn trong kho.
    /// </summary>
    Task<bool> BookAvailableAsync(
        int bookId);

    /// <summary>
    /// Kiểm tra cư dân có phiếu quá hạn.
    /// </summary>
    Task<bool> HasOverdueBorrowAsync(
        int residentId);

    /// <summary>
    /// Kiểm tra cư dân đang mượn đầu sách này.
    /// </summary>
    Task<bool> IsBookBorrowingAsync(
        int residentId,
        int bookId);

    #endregion
}