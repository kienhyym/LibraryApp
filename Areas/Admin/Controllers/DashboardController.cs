using Microsoft.AspNetCore.Mvc;
using LibraryApp.Areas.Admin.Services;

namespace LibraryApp.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(int? year)
    {
        int selectedYear = year ?? 2026;

        var model = await _dashboardService

            .GetDashboardAsync(selectedYear);


        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> BorrowChart(int year)
    {
        var chart = await _dashboardService.GetBorrowChartAsync(year);

        return Json(new
        {
            labels = chart.Labels,
            values = chart.Values,
            year = chart.Year
        });
    }
}