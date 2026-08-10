using LibraryApp.ViewModels.Information;

namespace LibraryApp.Services.Interfaces;

public interface IInformationService
{
    Task<InformationViewModel?> GetInformationAsync(
        int accountId);

    Task<InformationUpdateViewModel?>
        GetInformationForUpdateAsync(
            int accountId);

    Task<(bool Success, string Message)>
        UpdateInformationAsync(
            int accountId,
            InformationUpdateViewModel model);
}