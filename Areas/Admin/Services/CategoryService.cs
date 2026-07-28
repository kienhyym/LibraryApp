using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class CategoryService : ICategoryService
{
    private readonly LibDbContext _context;

    public CategoryService(LibDbContext context)
    {
        _context = context;
    }

    #region Get List

    public async Task<PaginatedList<CategoryViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize)
    {
        var query = _context.Categories.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>
                x.CategoryName.Contains(keyword));
        }

        // Sort
        var result = query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new CategoryViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryDescription = x.CategoryDescription,
                CreatedAt = x.CreatedAt
            });

        return await PaginatedList<CategoryViewModel>
            .CreateAsync(result, page, pageSize);
    }

    #endregion

    #region Get By Id

    public async Task<CategoryViewModel?> GetByIdAsync(int id)
    {
        return await _context.Categories

            .Where(x => x.CategoryId == id)

            .Select(x => new CategoryViewModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryDescription = x.CategoryDescription,
                CreatedAt = x.CreatedAt
            })

            .FirstOrDefaultAsync();
    }

    #endregion

    #region Create

    public async Task CreateAsync(CategoryViewModel model)
    {
        var category = new Category
        {
            CategoryName = model.CategoryName.Trim(),
            CategoryDescription = model.CategoryDescription?.Trim()
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Update

    public async Task UpdateAsync(CategoryViewModel model)
    {
        var category = await _context.Categories
            .FindAsync(model.CategoryId);

        if (category == null)
            throw new Exception("Không tìm thấy thể loại.");

        category.CategoryName = model.CategoryName.Trim();

        category.CategoryDescription =
            model.CategoryDescription?.Trim();

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Delete

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories
            .FindAsync(id);

        if (category == null)
            return;

        var isUsed = await _context.Books
            .AnyAsync(x => x.CategoryId == id);

        if (isUsed)
            throw new InvalidOperationException(
                "Thể loại đang được sử dụng, không thể xóa.");

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }

    #endregion

    #region Validation

    public async Task<bool> CategoryNameExistsAsync(
        string categoryName)
    {
        categoryName = categoryName.Trim();

        return await _context.Categories
            .AnyAsync(x =>
                x.CategoryName == categoryName);
    }

    public async Task<bool> CategoryNameExistsForUpdateAsync(
        string categoryName,
        int categoryId)
    {
        categoryName = categoryName.Trim();

        return await _context.Categories
            .AnyAsync(x =>
                x.CategoryName == categoryName &&
                x.CategoryId != categoryId);
    }

    #endregion
}