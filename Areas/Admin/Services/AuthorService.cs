using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryApp.Services;

public class AuthorService : IAuthorService
{
    private readonly LibDbContext _context;

    public AuthorService(LibDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedList<AuthorViewModel>> GetPagedAsync(
     string? keyword,
     int page,
     int pageSize)
    {
        var query = _context.Authors.AsQueryable();

        // Tìm kiếm
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>
                x.AuthorName.Contains(keyword));
        }

        // Sắp xếp
        var result = query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new AuthorViewModel
            {
                AuthorId = x.AuthorId,
                AuthorName = x.AuthorName,
                Nationality = x.Nationality,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt
            });

        return await PaginatedList<AuthorViewModel>
            .CreateAsync(result, page, pageSize);
    }

    public async Task<AuthorViewModel?> GetByIdAsync(int id)
    {
        return await _context.Authors
            .Where(x => x.AuthorId == id)
            .Select(x => new AuthorViewModel
            {
                AuthorId = x.AuthorId,
                AuthorName = x.AuthorName,
                Nationality = x.Nationality,
                Notes = x.Notes
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(AuthorViewModel model)
    {
        var author = new Author
        {
            AuthorName = model.AuthorName,
            Nationality = model.Nationality,
            Notes = model.Notes
        };

        _context.Authors.Add(author);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("Tên tác giả đã tồn tại.");
        }
    }

    public async Task UpdateAsync(AuthorViewModel model)
    {
        var author = await _context.Authors.FindAsync(model.AuthorId);

        if (author == null)
            throw new Exception("Không tìm thấy tác giả.");

        author.AuthorName = model.AuthorName.Trim();
        author.Nationality = model.Nationality?.Trim();
        author.Notes = model.Notes?.Trim();

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int authorId)
    {
        var author = await _context.Authors.FindAsync(authorId);

        if (author == null)
            return false;

        _context.Authors.Remove(author);

        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<bool> AuthorNameExistsAsync(string authorName)

    {

        return await _context.Authors.AnyAsync(x =>

            x.AuthorName == authorName);

    }
    public async Task<bool> AuthorNameExistsForUpdateAsync(

    string authorName,

    int authorId)

    {

        return await _context.Authors.AnyAsync(x =>

            x.AuthorName == authorName &&

            x.AuthorId != authorId);

    }
}