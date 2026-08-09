using LibraryApp.Common;
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

    public async Task<BorrowHistoryViewModel> GetBorrowHistoryAsync(
        int accountId,
        BorrowHistoryFilter filter)
    {
        var residentId = await _context.Residents
            .Where(x => x.AccountId == accountId)
            .Select(x => x.ResidentId)
            .FirstOrDefaultAsync();

        var query = _context.Borrowrecords

            .AsNoTracking()

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .Where(x => x.ResidentId == residentId);

        switch (filter.Status)
        {
            case "borrowing":

                query = query.Where(x =>
                    x.Borrowrecorddetails.Any(d =>
                        d.ReturnDate == null));

                break;

            case "returned":

                query = query.Where(x =>
                    x.Borrowrecorddetails.All(d =>
                        d.ReturnDate != null));

                break;

            case "overdue":

                query = query.Where(x =>
                    x.DueDate < DateTime.Now &&
                    x.Borrowrecorddetails.Any(d =>
                        d.ReturnDate == null));

                break;
        }

        var total = await query.CountAsync();

        var records = await query

            .OrderByDescending(x => x.BorrowDate)

            .Skip((filter.Page - 1) * PageSize)

            .Take(PageSize)

            .ToListAsync();

        var items = records.Select(x => new BorrowRecordViewModel
        {
            BorrowRecordId = x.BorrowRecordId,

            BorrowDate = x.BorrowDate,

            DueDate = x.DueDate,

            TotalBooks = x.Borrowrecorddetails.Count,

            Status =
                x.Borrowrecorddetails.All(d => d.ReturnDate != null)
                    ? "Đã trả"
                    : x.DueDate < DateTime.Now
                        ? "Quá hạn"
                        : "Đang mượn",

            Books = x.Borrowrecorddetails
                .Select(d => new BorrowHistoryBookViewModel
                {
                    BookId = d.BookId,

                    BookTitle = d.Book.Title,

                    CoverImage = d.Book.CoverImage,

                    ReturnDate = d.ReturnDate
                })
                .ToList()

        }).ToList();

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

    public async Task<BorrowRecordViewModel?> GetBorrowRecordAsync(
        int accountId,
        int borrowRecordId)
    {
        var residentId = await _context.Residents
            .Where(x => x.AccountId == accountId)
            .Select(x => x.ResidentId)
            .FirstOrDefaultAsync();

        var record = await _context.Borrowrecords

            .AsNoTracking()

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .FirstOrDefaultAsync(x =>
                x.BorrowRecordId == borrowRecordId &&
                x.ResidentId == residentId);

        if (record == null)
        {
            return null;
        }

        return new BorrowRecordViewModel
        {
            BorrowRecordId = record.BorrowRecordId,

            BorrowDate = record.BorrowDate,

            DueDate = record.DueDate,

            TotalBooks = record.Borrowrecorddetails.Count,

            Status =
                record.Borrowrecorddetails.All(d => d.ReturnDate != null)
                    ? "Đã trả"
                    : record.DueDate < DateTime.Now
                        ? "Quá hạn"
                        : "Đang mượn",

            Books = record.Borrowrecorddetails
                .Select(d => new BorrowHistoryBookViewModel
                {
                    BookId = d.BookId,

                    BookTitle = d.Book.Title,

                    CoverImage = d.Book.CoverImage,

                    ReturnDate = d.ReturnDate,

                    ReturnStatus = d.ReturnStatus
                })
                .ToList()
        };
    }
}