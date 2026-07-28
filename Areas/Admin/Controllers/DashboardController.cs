using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _dashboardService.GetDashboardAsync();

        return View(vm);
    }
}