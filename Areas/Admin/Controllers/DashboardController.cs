using Microsoft.AspNetCore.Mvc;
using LibraryApp.Areas.Admin.Services;
using Microsoft.AspNetCore.Authorization;

namespace LibraryApp.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
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