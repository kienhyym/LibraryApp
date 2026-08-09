using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Areas.Admin.ViewModels.Dashboard;

namespace LibraryApp.Areas.Admin.Services;

public interface IDashboardService

{
    Task<DashboardViewModel> GetDashboardAsync(int year);
    
    Task<BorrowChartViewModel> GetBorrowChartAsync(int year);

}