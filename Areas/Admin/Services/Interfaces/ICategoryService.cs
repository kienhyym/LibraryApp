using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;

namespace LibraryApp.Services.Interfaces;

public interface ICategoryService
{
    // Danh sách + tìm kiếm + phân trang
    Task<PaginatedList<CategoryViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize);

    // Lấy theo Id
    Task<CategoryViewModel?> GetByIdAsync(int id);

    // Thêm mới
    Task CreateAsync(CategoryViewModel model);

    // Cập nhật
    Task UpdateAsync(CategoryViewModel model);

    // Xóa
    Task DeleteAsync(int id);

    // Kiểm tra trùng tên khi thêm
    Task<bool> CategoryExistsByNameAsync(string categoryName);

    // Kiểm tra trùng tên khi sửa
    Task<bool> CategoryExistsByNameForUpdateAsync(
        string categoryName,
        int categoryId);
}