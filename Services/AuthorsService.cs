using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Author;

using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class AuthorsService : IAuthorsService
{
    private readonly LibDbContext _context;

    public AuthorsService(
        LibDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorsViewModel>> GetAuthorsAsync()
    {
        return await _context.Authors

            .AsNoTracking()

            .OrderBy(x => x.AuthorName)

            .Select(x => new AuthorsViewModel
            {
                AuthorId = x.AuthorId,

                AuthorName = x.AuthorName,

                Nationality = x.Nationality,

                Notes = x.Notes,

                TotalBooks = x.Books.Count()
            })

            .ToListAsync();
    }
}