using LibraryApp.Services.Interfaces;
using LibraryApp.Models;
using LibraryApp.ViewModels.Category;

using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class CategoriesService : ICategoriesService
{
    private readonly LibDbContext _context;

    public CategoriesService(
        LibDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoriesViewModel>> GetCategoriesAsync()
    {
        return await _context.Categories

            .AsNoTracking()

            .OrderBy(x => x.CategoryName)

            .Select(x => new CategoriesViewModel
            {
                CategoryId = x.CategoryId,

                CategoryName = x.CategoryName,

                Description = x.CategoryDescription,

                TotalBooks = x.Books.Count()
            })

            .ToListAsync();
    }
}