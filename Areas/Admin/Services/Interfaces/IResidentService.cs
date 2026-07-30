using LibraryApp.Areas.Admin.ViewModels.Resident;
using LibraryApp.Common;

namespace LibraryApp.Services.Interfaces;

public interface IResidentService
{
    #region Query

    Task<PaginatedList<ResidentListViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize);

    Task<ResidentCreateViewModel> GetCreateModelAsync();

    Task<ResidentEditViewModel?> GetEditModelAsync(int id);

    Task<ResidentDetailViewModel?> GetByIdAsync(int id);

    #endregion

    #region Registration

    Task SendOtpAsync(
    ResidentCreateViewModel model);

    Task ResendOtpAsync(

    string email);

    Task VerifyOtpAndCreateAsync(
        ResidentCreateViewModel resident,
        ResidentVerifyOtpViewModel otp);

    #endregion

    #region CRUD

    Task UpdateAsync(ResidentEditViewModel model);

    Task ToggleActiveAsync(int residentId);

    #endregion

    #region Validation

    Task<bool> ResidentExistsByEmailAsync(string email);

    Task<bool> ResidentExistsByPhoneAsync(string? phoneNumber);

    Task<bool> ResidentExistsByEmailForUpdateAsync(
        string email,
        int residentId);

    Task<bool> ResidentExistsByPhoneForUpdateAsync(
        string? phoneNumber,
        int residentId);

    #endregion
}