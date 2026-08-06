using LibraryApp.ViewModels.Profile;

namespace LibraryApp.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel?> GetProfileAsync(int accountId);

    Task<ProfileUpdateViewModel?> GetProfileForUpdateAsync(int accountId);

    Task<(bool Success, string Message)> UpdateProfileAsync(
        int accountId,
        ProfileUpdateViewModel model);

    Task<(bool Success, string Message)> ChangePasswordAsync(
    int accountId,
    ChangePasswordViewModel model);
}