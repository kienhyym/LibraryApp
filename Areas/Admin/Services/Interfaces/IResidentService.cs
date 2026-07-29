using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;

namespace LibraryApp.Services.Interfaces;

public interface IResidentService
{
    #region Query

    // Danh sách + tìm kiếm + phân trang
    Task<PaginatedList<ResidentViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize);

    // Model cho Create
    Task<ResidentViewModel> GetCreateModelAsync();

    // Model cho Edit
    Task<ResidentViewModel?> GetEditModelAsync(int id);

    // Chi tiết
    Task<ResidentViewModel?> GetByIdAsync(int id);

    #endregion

    #region OTP

    // Gửi OTP
    Task SendOtpAsync(ResidentViewModel model);

    // Gửi lại OTP
    Task ResendOtpAsync(string email);

    // Kiểm tra email đã xác thực OTP hay chưa
    Task<bool> IsOtpVerifiedAsync(string email);

    // Xác thực OTP + tạo Account + Resident
    Task VerifyOtpAndCreateAsync(
        ResidentViewModel model);

    #endregion

    #region CRUD

    Task UpdateAsync(ResidentViewModel model);

    Task ToggleActiveAsync(int residentId);

    #endregion

    #region Validation

    Task<bool> ResidentExistsByEmailAsync(
        string email);

    Task<bool> ResidentExistsByPhoneAsync(
        string phoneNumber);

    Task<bool> ResidentExistsByEmailForUpdateAsync(
        string email,
        int residentId);

    Task<bool> ResidentExistsByPhoneForUpdateAsync(
        string phoneNumber,
        int residentId);

    #endregion
}