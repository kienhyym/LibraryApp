using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}