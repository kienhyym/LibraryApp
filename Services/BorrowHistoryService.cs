using LibraryApp.Common;
using LibraryApp.Enums;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.BorrowHistory;

using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class BorrowHistoryService : IBorrowHistoryService
{
    private readonly LibDbContext _context;

    private const int PageSize = 10;

    public BorrowHistoryService(
        LibDbContext context)
    {
        _context = context;
    }


    #region Borrow History


    public async Task<BorrowHistoryViewModel> GetBorrowHistoryAsync(
        int accountId,
        BorrowHistoryFilter filter)
    {
        // ==========================================
        // Lấy ResidentId
        // ==========================================

        var residentId = await _context.Residents

            .Where(x =>
                x.AccountId == accountId)

            .Select(x =>
                x.ResidentId)

            .FirstOrDefaultAsync();


        // ==========================================
        // Query
        // ==========================================

        var query = _context.Borrowrecords

            .AsNoTracking()

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .Where(x =>
                x.ResidentId == residentId);


        // ==========================================
        // Filter trạng thái
        // ==========================================

        switch (filter.Status)
        {
            case "borrowing":

                query = query.Where(x =>
                    x.BorrowRecordStatus ==
                    BorrowRecordStatus.Borrowing);

                break;


            case "returned":

                query = query.Where(x =>
                    x.BorrowRecordStatus ==
                    BorrowRecordStatus.Completed);

                break;


            case "overdue":

                query = query.Where(x =>
                    x.BorrowRecordStatus ==
                    BorrowRecordStatus.Overdue);

                break;
        }


        // ==========================================
        // Tổng số phiếu
        // ==========================================

        var total =
            await query.CountAsync();


        // ==========================================
        // Phân trang
        // ==========================================

        var records = await query

            .OrderByDescending(x =>
                x.BorrowDate)

            .Skip(
                (filter.Page - 1)
                * PageSize)

            .Take(PageSize)

            .ToListAsync();


        // ==========================================
        // Mapping ViewModel
        // ==========================================

        var items = records

            .Select(x => new BorrowRecordViewModel
            {
                BorrowRecordId =
                    x.BorrowRecordId,

                BorrowDate =
                    x.BorrowDate,

                DueDate =
                    x.DueDate,

                // Ngày trả nằm ở BORROWRECORDS
                ReturnDate =
                    x.ReturnDate,

                TotalBooks =
                    x.Borrowrecorddetails.Count,

                Status =
                    GetStatusText(x),

                Books =
                    x.Borrowrecorddetails

                        .Select(d =>
                            new BorrowHistoryBookViewModel
                            {
                                BookId =
                                    d.BookId,

                                BookTitle =
                                    d.Book.Title,

                                CoverImage =
                                    d.Book.CoverImage,

                                // Ngày trả của toàn bộ phiếu
                                ReturnDate =
                                    x.ReturnDate,

                                ReturnStatus =
                                    d.ReturnStatus
                            })

                        .ToList()

            })

            .ToList();


        // ==========================================
        // Return
        // ==========================================

        return new BorrowHistoryViewModel
        {
            Filter = filter,

            BorrowRecords =
                new PaginatedList<BorrowRecordViewModel>(
                    items,
                    total,
                    filter.Page,
                    PageSize)
        };
    }


    #endregion


    #region Borrow Record Detail


    public async Task<BorrowRecordViewModel?> GetBorrowRecordAsync(
        int accountId,
        int borrowRecordId)
    {
        // ==========================================
        // Lấy ResidentId
        // ==========================================

        var residentId = await _context.Residents

            .Where(x =>
                x.AccountId == accountId)

            .Select(x =>
                x.ResidentId)

            .FirstOrDefaultAsync();


        // ==========================================
        // Lấy phiếu mượn
        // ==========================================

        var record = await _context.Borrowrecords

            .AsNoTracking()

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .FirstOrDefaultAsync(x =>
                x.BorrowRecordId ==
                    borrowRecordId

                &&

                x.ResidentId ==
                    residentId);


        if (record == null)
        {
            return null;
        }


        // ==========================================
        // Mapping
        // ==========================================

        return new BorrowRecordViewModel
        {
            BorrowRecordId =
                record.BorrowRecordId,

            BorrowDate =
                record.BorrowDate,

            DueDate =
                record.DueDate,

            // Ngày trả nằm ở BORROWRECORDS
            ReturnDate =
                record.ReturnDate,

            TotalBooks =
                record.Borrowrecorddetails.Count,

            Status =
                GetStatusText(record),

            Books =
                record.Borrowrecorddetails

                    .Select(d =>
                        new BorrowHistoryBookViewModel
                        {
                            BookId =
                                d.BookId,

                            BookTitle =
                                d.Book.Title,

                            CoverImage =
                                d.Book.CoverImage,

                            // Dùng ReturnDate của phiếu
                            ReturnDate =
                                record.ReturnDate,

                            ReturnStatus =
                                d.ReturnStatus
                        })

                    .ToList()
        };
    }


    #endregion


    #region Status


    private static string GetStatusText(
        Borrowrecord record)
    {
        return record.BorrowRecordStatus switch
        {
            BorrowRecordStatus.Borrowing
                => "Đang mượn",

            BorrowRecordStatus.Completed
                => "Đã trả",

            BorrowRecordStatus.Overdue
                => "Quá hạn",

            _ => "Không xác định"
        };
    }


    #endregion
}