using System.Security.Claims;

using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.BorrowHistory;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers;

[Authorize]
public class BorrowHistoryController : Controller
{
    private readonly IBorrowHistoryService _borrowHistoryService;

    public BorrowHistoryController(
        IBorrowHistoryService borrowHistoryService)
    {
        _borrowHistoryService = borrowHistoryService;
    }

    /// <summary>
    /// Danh sách phiếu mượn
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        BorrowHistoryFilter filter)
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = await _borrowHistoryService
            .GetBorrowHistoryAsync(
                accountId,
                filter);

        return View(model);
    }

    /// <summary>
    /// Chi tiết phiếu mượn
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Detail(
        int id)
    {
        var accountId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var model = await _borrowHistoryService
            .GetBorrowRecordAsync(
                accountId,
                id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }
}