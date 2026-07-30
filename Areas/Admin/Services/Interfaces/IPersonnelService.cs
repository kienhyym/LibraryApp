using LibraryApp.Areas.Admin.ViewModels.Personnel;
using LibraryApp.Common;

namespace LibraryApp.Services.Interfaces;

public interface IPersonnelService
{
    #region Query

    Task<PaginatedList<PersonnelListViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize);

    Task<PersonnelCreateViewModel> GetCreateModelAsync();

    Task<PersonnelEditViewModel?> GetEditModelAsync(int id);

    Task<PersonnelDetailViewModel?> GetByIdAsync(int id);

    #endregion

    #region CRUD

    Task CreateAsync(PersonnelCreateViewModel model);

    Task UpdateAsync(PersonnelEditViewModel model);

    Task ToggleActiveAsync(int personnelId);

    #endregion

    #region Validation

    Task<bool> PersonnelExistsByEmailAsync(string email);

    Task<bool> PersonnelExistsByPhoneAsync(string? phoneNumber);

    Task<bool> PersonnelExistsByEmailForUpdateAsync(
        string email,
        int personnelId);

    Task<bool> PersonnelExistsByPhoneForUpdateAsync(
        string? phoneNumber,
        int personnelId);

    #endregion
}