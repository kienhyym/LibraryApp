using LibraryApp.Areas.Admin.ViewModels;

namespace LibraryApp.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}