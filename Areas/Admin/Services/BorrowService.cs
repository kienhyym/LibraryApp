using LibraryApp.Areas.Admin.ViewModels.Borrow;
using LibraryApp.Common;
using LibraryApp.Enums;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class BorrowService : IBorrowService
{
    private readonly LibDbContext _context;

    public BorrowService(
        LibDbContext context)
    {
        _context = context;
    }

    #region Query

    public async Task<PaginatedList<BorrowListViewModel>> GetPagedAsync(
    string? keyword,
    BorrowRecordStatus? status,
    int page,
    int pageSize)
{
    // ==========================================
    // Cập nhật các phiếu quá hạn
    // ==========================================

    await UpdateOverdueAsync();


    // ==========================================
    // Query phiếu mượn
    // ==========================================

    var query = _context.Borrowrecords
        .AsNoTracking()
        .Include(x => x.Resident)
        .Include(x => x.Personnel)
        .Include(x => x.Borrowrecorddetails)
        .AsQueryable();


    // ==========================================
    // Tìm kiếm
    // ==========================================

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        keyword = keyword.Trim();

        query = query.Where(x =>
            x.Resident.FullName.Contains(keyword));
    }


    // ==========================================
    // Lọc theo trạng thái
    //
    // 1 = Borrowing
    // 2 = Completed
    // 3 = Overdue
    // ==========================================

    if (status.HasValue)
    {
        query = query.Where(x =>
            x.BorrowRecordStatus == status.Value);
    }


    // ==========================================
    // Projection
    // ==========================================

    var result = query
        .OrderByDescending(x => x.BorrowDate)
        .Select(x => new BorrowListViewModel
        {
            BorrowRecordId =
                x.BorrowRecordId,

            ResidentName =
                x.Resident.FullName,

            BorrowDate =
                x.BorrowDate,

            DueDate =
                x.DueDate,

            // Ngày trả nằm ở BORROWRECORDS
            ReturnDate =
                x.ReturnDate,

            BorrowRecordStatus =
                x.BorrowRecordStatus,

            TotalBooks =
                x.Borrowrecorddetails.Count
        });


    // ==========================================
    // Phân trang
    // ==========================================

    return await PaginatedList<BorrowListViewModel>
        .CreateAsync(
            result,
            page,
            pageSize);
}

    public Task<BorrowCreateViewModel> GetCreateModelAsync()
    {
        return Task.FromResult(
            new BorrowCreateViewModel
            {
                DueDate = DateOnly.FromDateTime(
                    DateTime.Today.AddDays(14))
            });
    }

    public async Task<BorrowDetailViewModel?> GetDetailAsync(
        int borrowRecordId)
    {
        await UpdateOverdueAsync();
        return await _context.Borrowrecords

            .Include(x => x.Resident)

            .Include(x => x.Personnel)

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .ThenInclude(x => x.Authors)

            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)

            .ThenInclude(x => x.Category)

            .Where(x => x.BorrowRecordId == borrowRecordId)

            .Select(x => new BorrowDetailViewModel
            {
                BorrowRecordId = x.BorrowRecordId,

                ResidentName = x.Resident.FullName,

                PersonnelName = x.Personnel.FullName,

                ReturnPersonnelName =
                x.ReturnPersonnel != null
                    ? x.ReturnPersonnel.FullName
                    : null,

                BorrowDate = x.BorrowDate,

                DueDate = x.DueDate,
                
                ReturnDate = x.ReturnDate,
                
                BorrowRecordStatus = x.BorrowRecordStatus,

                Notes = x.Notes,

                Books = x.Borrowrecorddetails
    .Select(d => new BorrowBookItemViewModel
    {
        BorrowRecordDetailId = d.BorrowRecordDetailId,

        BookId = d.BookId,

        Title = d.Book.Title,

        AuthorName = string.Join(
            ", ",
            d.Book.Authors
                .OrderBy(a => a.AuthorName)
                .Select(a => a.AuthorName)),

        CategoryName = d.Book.Category.CategoryName,

        AvailableQuantity = d.Book.AvailableQuantity,

        ReturnStatus = d.ReturnStatus == null

            ? null

            : (ReturnStatus)d.ReturnStatus.Value,

        ReturnNote = d.ReturnNote,
        Penalty = d.Penalty

    })
                    .ToList()
            })

            .FirstOrDefaultAsync();
    }

    #endregion


    #region Lookup

    public async Task<List<ResidentLookupViewModel>> SearchResidentsAsync(
        string? keyword)
    {
        keyword = keyword?.Trim();

        var query = _context.Residents
            .Include(x => x.Account)
            .Where(x => x.Account.IsActive);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>

                x.FullName.Contains(keyword)

                || x.Account.Email.Contains(keyword)

                || (x.PhoneNumber != null &&
                    x.PhoneNumber.Contains(keyword)));
        }

        return await query
            .OrderBy(x => x.FullName)
            .Take(20)
            .Select(x => new ResidentLookupViewModel
            {
                ResidentId = x.ResidentId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber
            })
            .ToListAsync();
    }

    public async Task<List<BookLookupViewModel>> SearchBooksAsync(
        string? keyword)
    {
        keyword = keyword?.Trim();

        var query = _context.Books
            .Include(x => x.Authors)
            .Include(x => x.Category)
            .Where(x =>

                x.IsAvailable

                && x.AvailableQuantity > 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>

                x.Title.Contains(keyword)

                || x.Authors.Any(a =>
    a.AuthorName.Contains(keyword))

                || x.Category.CategoryName.Contains(keyword));
        }

        return await query
            .OrderBy(x => x.Title)
            .Take(20)
            .Select(x => new BookLookupViewModel
            {
                BookId = x.BookId,

                Title = x.Title,

                AuthorName = string.Join(
                ", ",
                x.Authors
                    .OrderBy(a => a.AuthorName)
                    .Select(a => a.AuthorName)),

                CategoryName = x.Category.CategoryName,

                AvailableQuantity = x.AvailableQuantity
            })
            .ToListAsync();
    }

    #endregion


    #region Validation

    public async Task<bool> CanBorrowAsync(
     int residentId)
    {
        const int maxBorrowBooks = 5;

        var borrowingBooks =
            await _context.Borrowrecorddetails
                .Include(x => x.BorrowRecord)
                .CountAsync(x =>
                    x.BorrowRecord.ResidentId == residentId
                    &&
                    x.BorrowRecord.BorrowRecordStatus ==
                        BorrowRecordStatus.Borrowing);

        return borrowingBooks < maxBorrowBooks;
    }

    public async Task<bool> BookAvailableAsync(
        int bookId)
    {
        return await _context.Books
            .AnyAsync(x =>

                x.BookId == bookId

                && x.IsAvailable

                && x.AvailableQuantity > 0);
    }

    public async Task<bool> HasOverdueBorrowAsync(
        int residentId)
    {
        return await _context.Borrowrecords
            .AnyAsync(x =>

                x.ResidentId == residentId

                && x.BorrowRecordStatus ==
                    BorrowRecordStatus.Overdue);
    }

    public async Task<bool> IsBookBorrowingAsync(
    int residentId,
    int bookId)
    {
        return await _context.Borrowrecorddetails
            .Include(x => x.BorrowRecord)
            .AnyAsync(x =>

                x.BookId == bookId

                &&

                x.BorrowRecord.ResidentId == residentId

                &&

                x.BorrowRecord.BorrowRecordStatus ==
                    BorrowRecordStatus.Borrowing);
    }

    #endregion


    private async Task ValidateBorrowAsync(
    BorrowCreateViewModel model)
    {
        if (model.Books == null || !model.Books.Any())
        {
            throw new InvalidOperationException(
                "Vui lòng chọn ít nhất một quyển sách.");
        }

        var residentExists = await _context.Residents
            .AnyAsync(x => x.ResidentId == model.ResidentId);

        if (!residentExists)
        {
            throw new InvalidOperationException(
                "Cư dân không tồn tại.");
        }

        if (await HasOverdueBorrowAsync(model.ResidentId))
        {
            throw new InvalidOperationException(
                "Cư dân đang có phiếu mượn quá hạn.");
        }

        if (!await CanBorrowAsync(model.ResidentId))
        {
            throw new InvalidOperationException(
                "Cư dân đã đạt số lượng sách được phép mượn.");
        }

        if (model.Books
            .GroupBy(x => x.BookId)
            .Any(x => x.Count() > 1))
        {
            throw new InvalidOperationException(
                "Một quyển sách chỉ được chọn một lần.");
        }
    }

    private async Task AddBorrowDetailsAsync(
     Borrowrecord borrowRecord,
     BorrowCreateViewModel model,
     Dictionary<int, Book> books,
     HashSet<int> borrowingBookIds)
    {
        foreach (var item in model.Books)
        {
            if (!books.TryGetValue(item.BookId, out var book))
            {
                throw new InvalidOperationException(
                    $"Không tìm thấy sách \"{item.Title}\".");
            }

            if (!book.IsAvailable)
            {
                throw new InvalidOperationException(
                    $"Sách \"{book.Title}\" hiện không khả dụng.");
            }

            if (book.AvailableQuantity <= 0)
            {
                throw new InvalidOperationException(
                    $"Sách \"{book.Title}\" đã hết.");
            }

            if (borrowingBookIds.Contains(item.BookId))
            {
                throw new InvalidOperationException(
                    $"Cư dân đang mượn sách \"{book.Title}\".");
            }

            _context.Borrowrecorddetails.Add(
                new Borrowrecorddetail
                {
                    BorrowRecordId = borrowRecord.BorrowRecordId,
                    BookId = item.BookId,
                    Penalty = 0
                });

            book.AvailableQuantity--;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Borrowrecord> CreateBorrowRecordAsync(
    BorrowCreateViewModel model,
    int personnelId)
    {
        var borrowRecord = new Borrowrecord
        {
            ResidentId = model.ResidentId,

            PersonnelId = personnelId,

            BorrowDate = DateTime.Now,

            DueDate = model.DueDate
                .ToDateTime(TimeOnly.MinValue),

            Notes = model.Notes,

            BorrowRecordStatus =
                BorrowRecordStatus.Borrowing
        };

        _context.Borrowrecords.Add(borrowRecord);

        await _context.SaveChangesAsync();

        return borrowRecord;
    }
    private async Task<Dictionary<int, Book>> LoadBooksAsync(
    BorrowCreateViewModel model)
    {
        var bookIds = model.Books
            .Select(x => x.BookId)
            .ToList();

        return await _context.Books
            .Where(x => bookIds.Contains(x.BookId))
            .ToDictionaryAsync(x => x.BookId);
    }


    private async Task<HashSet<int>> LoadBorrowingBooksAsync(
    int residentId)
    {
        return await _context.Borrowrecorddetails
            .Include(x => x.BorrowRecord)
            .Where(x =>

                x.BorrowRecord.ResidentId == residentId

                &&

                x.BorrowRecord.BorrowRecordStatus ==
                    BorrowRecordStatus.Borrowing)

            .Select(x => x.BookId)

            .ToHashSetAsync();
    }


    public async Task CreateAsync(
    BorrowCreateViewModel model,
    int personnelId)
    {
        await ValidateBorrowAsync(model);

        var books =
            await LoadBooksAsync(model);

        var borrowingBooks =
            await LoadBorrowingBooksAsync(
                model.ResidentId);

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var borrowRecord =
                await CreateBorrowRecordAsync(
                    model,
                    personnelId);

            await AddBorrowDetailsAsync(
                borrowRecord,
                model,
                books,
                borrowingBooks);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #region Return

    public async Task ReturnBooksAsync(
        int borrowRecordId,
        List<BorrowReturnItemViewModel> books,
        int returnPersonnelId)
    {
        await UpdateOverdueAsync();

        if (books == null || books.Count == 0)
        {
            throw new InvalidOperationException(
                "Phiếu mượn không có sách.");
        }

        var borrowRecord = await _context.Borrowrecords
            .Include(x => x.Borrowrecorddetails)
                .ThenInclude(x => x.Book)
            .FirstOrDefaultAsync(x =>
                x.BorrowRecordId == borrowRecordId);

        if (borrowRecord == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy phiếu mượn.");
        }

        if (borrowRecord.BorrowRecordStatus ==
            BorrowRecordStatus.Completed)
        {
            throw new InvalidOperationException(
                "Phiếu mượn này đã được trả.");
        }

        // ==========================================
        // Kiểm tra nhân viên nhận trả
        // ==========================================

        var personnelExists =
            await _context.Personnel
                .AnyAsync(x =>
                    x.PersonnelId == returnPersonnelId);

        if (!personnelExists)
        {
            throw new InvalidOperationException(
                "Nhân viên nhận trả không tồn tại.");
        }

        // ==========================================
        // Lấy toàn bộ chi tiết phiếu
        // ==========================================

        var details =
            borrowRecord.Borrowrecorddetails.ToList();

        // ==========================================
        // Bắt buộc phải trả toàn bộ sách
        // ==========================================

        if (books.Count != details.Count)
        {
            throw new InvalidOperationException(
                "Phải trả toàn bộ sách trong phiếu mượn.");
        }

        var detailIds = books
            .Select(x => x.BorrowRecordDetailId)
            .ToHashSet();

        if (details.Any(x =>
            !detailIds.Contains(
                x.BorrowRecordDetailId)))
        {
            throw new InvalidOperationException(
                "Danh sách sách trả không khớp với phiếu mượn.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // ==========================================
            // Xử lý từng sách
            // ==========================================

            foreach (var detail in details)
            {
                var item = books.First(x =>
                    x.BorrowRecordDetailId ==
                    detail.BorrowRecordDetailId);

                // ======================================
                // Kiểm tra ReturnStatus
                // ======================================

                if (!Enum.IsDefined(
                        typeof(ReturnStatus),
                        item.ReturnStatus))
                {
                    throw new InvalidOperationException(
                        "Tình trạng trả sách không hợp lệ.");
                }

                // ======================================
                // Kiểm tra tiền phạt
                // ======================================

                if (item.Penalty < 0 ||
                    item.Penalty > 10_000_000)
                {
                    throw new InvalidOperationException(
                        "Tiền phạt phải từ 0 đến 10.000.000 VNĐ.");
                }

                // ======================================
                // Sách tốt → không phạt
                // ======================================

                if (item.ReturnStatus ==
                    ReturnStatus.Returned)
                {
                    item.Penalty = 0;
                }

                // ======================================
                // Cập nhật chi tiết
                // ======================================

                detail.ReturnStatus =
                    item.ReturnStatus;

                detail.ReturnNote =
                    item.ReturnNote?.Trim();

                detail.Penalty =
                    item.Penalty;

                // ======================================
                // Cập nhật số lượng sách
                // ======================================

                if (item.ReturnStatus ==
                    ReturnStatus.Returned)
                {
                    detail.Book.AvailableQuantity++;
                }

                // Lost / Damaged
                // Không cộng lại kho
            }

            // ==========================================
            // CẬP NHẬT PHIẾU MƯỢN
            // ==========================================

            borrowRecord.ReturnDate =
                DateTime.Now;

            borrowRecord.ReturnPersonnelId =
                returnPersonnelId;

            borrowRecord.BorrowRecordStatus =
                BorrowRecordStatus.Completed;

            // ==========================================
            // Lưu
            // ==========================================

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    #endregion

    private async Task UpdateOverdueAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var overdueRecords = await _context.Borrowrecords
            .Where(x =>
                x.BorrowRecordStatus == BorrowRecordStatus.Borrowing &&
                x.DueDate.Date < DateTime.Today)
            .ToListAsync();

        if (!overdueRecords.Any())
            return;

        foreach (var record in overdueRecords)
        {
            record.BorrowRecordStatus =
                BorrowRecordStatus.Overdue;
        }

        await _context.SaveChangesAsync();
    }
}