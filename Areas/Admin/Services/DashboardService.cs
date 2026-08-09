using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Enums;
using LibraryApp.Areas.Admin.ViewModels.Dashboard;

namespace LibraryApp.Areas.Admin.Services;

public class DashboardService : IDashboardService
{
    private readonly LibDbContext _context;

    public DashboardService(LibDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(int year)
    {
        var model = new DashboardViewModel();

        await LoadStatistics(model);

        await LoadBorrowChart(model, year);

        await LoadTopBooks(model, year);

        await LoadTopCategories(model, year);

        await LoadTopAuthors(model, year);

        await LoadDueBorrows(model);

        return model;
    }

    #region Statistics

    private async Task LoadStatistics(
    DashboardViewModel model)
    {
        model.Statistics.TotalBooks =
            await _context.Books.CountAsync();

        model.Statistics.TotalAuthors =
            await _context.Authors.CountAsync();

        model.Statistics.TotalCategories =
            await _context.Categories.CountAsync();

        model.Statistics.TotalResidents =
            await _context.Residents.CountAsync();

        model.Statistics.TotalBorrowRecords =
            await _context.Borrowrecords.CountAsync();

        model.Statistics.BorrowingRecords =
            await _context.Borrowrecords
                .CountAsync(x =>
                    x.BorrowRecordStatus == BorrowRecordStatus.Borrowing);

        model.Statistics.OverdueRecords =
            await _context.Borrowrecords
                .CountAsync(x =>
                    x.BorrowRecordStatus == BorrowRecordStatus.Borrowing
                    && x.DueDate.Date < DateTime.Today);
    }

    #endregion

    #region Borrow Chart

    private async Task LoadBorrowChart(
    DashboardViewModel model,
    int year)
    {
        model.BorrowChart.Year = year;

        // Labels từ tháng 1 -> 12
        model.BorrowChart.Labels = new List<string>
    {
        "T1",
        "T2",
        "T3",
        "T4",
        "T5",
        "T6",
        "T7",
        "T8",
        "T9",
        "T10",
        "T11",
        "T12"
    };

        model.BorrowChart.Values = Enumerable
            .Repeat(0, 12)
            .ToList();

        var chartData = await _context.Borrowrecords
            .AsNoTracking()
            .Where(x => x.BorrowDate.Year == year)
            .GroupBy(x => x.BorrowDate.Month)
            .Select(g => new
            {
                Month = g.Key,
                Total = g.Count()
            })
            .ToListAsync();

        foreach (var item in chartData)
        {
            model.BorrowChart.Values[item.Month - 1] =
                item.Total;
        }
    }

    #endregion

    #region Top Books

    private async Task LoadTopBooks(
     DashboardViewModel model,
     int year)
    {
        model.TopBooks = await _context.Borrowrecorddetails
            .AsNoTracking()

            .Where(x => x.BorrowRecord.BorrowDate.Year == year)

            .GroupBy(x => new
            {
                x.BookId,
                x.Book.Title
            })

            .Select(g => new TopItemViewModel
            {
                Id = g.Key.BookId,

                Name = g.Key.Title,

                BorrowCount = g.Count()
            })

            .OrderByDescending(x => x.BorrowCount)

            .ThenBy(x => x.Name)

            .Take(10)

            .ToListAsync();
    }

    #endregion
    private IQueryable<Borrowrecorddetail> BorrowDetailsOfYear(
    int year)
    {
        return _context.Borrowrecorddetails

            .AsNoTracking()

            .Where(x =>
                x.BorrowRecord.BorrowDate.Year == year);
    }

    #region Top Categories

    private async Task LoadTopCategories(
    DashboardViewModel model,
    int year)
    {
        model.TopCategories = await _context.Borrowrecorddetails

            .AsNoTracking()

            .Where(x => x.BorrowRecord.BorrowDate.Year == year)

            .GroupBy(x => new
            {
                x.Book.CategoryId,
                x.Book.Category.CategoryName
            })

            .Select(g => new TopItemViewModel
            {
                Id = g.Key.CategoryId,

                Name = g.Key.CategoryName,

                BorrowCount = g.Count()
            })

            .OrderByDescending(x => x.BorrowCount)

            .ThenBy(x => x.Name)

            .Take(10)

            .ToListAsync();
    }

    #endregion

    #region Top Authors

    private async Task LoadTopAuthors(
    DashboardViewModel model,
    int year)
    {
        model.TopAuthors = await _context.Borrowrecorddetails

            .AsNoTracking()

            .Where(x => x.BorrowRecord.BorrowDate.Year == year)

            .GroupBy(x => new
            {
                x.Book.AuthorId,
                x.Book.Author.AuthorName
            })

            .Select(g => new TopItemViewModel
            {
                Id = g.Key.AuthorId,

                Name = g.Key.AuthorName,

                BorrowCount = g.Count()
            })

            .OrderByDescending(x => x.BorrowCount)

            .ThenBy(x => x.Name)

            .Take(10)

            .ToListAsync();
    }

    #endregion

   #region Due Borrows

private async Task LoadDueBorrows(
    DashboardViewModel model)
{
    var today = DateTime.Today;

    model.DueBorrows = await _context.Borrowrecords

        .AsNoTracking()

        .Where(x =>
            x.BorrowRecordStatus == BorrowRecordStatus.Borrowing
            && x.DueDate.Date >= today
            && x.DueDate.Date <= today.AddDays(5))

        .OrderBy(x => x.DueDate)

        .Select(x => new DueBorrowViewModel
        {
            BorrowRecordId = x.BorrowRecordId,

            ResidentName = x.Resident.FullName,

            Email = x.Resident.Account.Email,

            DueDate = x.DueDate,

            RemainingDays =
                EF.Functions.DateDiffDay(
                    today,
                    x.DueDate),

            TotalBooks =
                x.Borrowrecorddetails.Count()
        })

        .ToListAsync();
}

#endregion

    public async Task<BorrowChartViewModel> GetBorrowChartAsync(int year)
    {
        var values = new List<int>();

        for (int month = 1; month <= 12; month++)
        {
            var count = await _context.Borrowrecords
                .CountAsync(x =>
                    x.BorrowDate.Year == year &&
                    x.BorrowDate.Month == month);

            values.Add(count);
        }

        return new BorrowChartViewModel
        {
            Year = year,

            Labels = new List<string>
        {
            "T1",
            "T2",
            "T3",
            "T4",
            "T5",
            "T6",
            "T7",
            "T8",
            "T9",
            "T10",
            "T11",
            "T12"
        },

            Values = values
        };
    }

}