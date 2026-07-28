using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class DashboardService : IDashboardService
{
    private readonly LibDbContext _context;

    public DashboardService(LibDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        return new DashboardViewModel
        {
            TotalBooks = await _context.Books.CountAsync(),

            TotalAuthors = await _context.Authors.CountAsync(),

            TotalCategories = await _context.Categories.CountAsync(),

            TotalResidents = await _context.Residents.CountAsync(),

            TotalBorrowRecords = await _context.Borrowrecords.CountAsync(),

            BorrowingBooks = await _context.Borrowrecords
                .CountAsync(x => x.BorrowRecordStatus == 1),

            OverdueBooks = await _context.Borrowrecords
                .CountAsync(x =>
                    x.BorrowRecordStatus == 1 &&
                    x.DueDate < DateTime.Now)
        };
    }
}