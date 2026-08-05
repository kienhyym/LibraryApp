using LibraryApp.Areas.Admin.ViewModels.Borrow;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class BorrowController : AdminBaseController
{
    private readonly IBorrowService _borrowService;

    public BorrowController(
        IBorrowService borrowService)
    {
        _borrowService = borrowService;
    }

    #region Index

    public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
    {
        const int pageSize = 10;

        var model = await _borrowService.GetPagedAsync(
            keyword,
            page,
            pageSize);

        ViewBag.Keyword = keyword;

        return View(model);
    }

    #endregion

    #region Create

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model =
            await _borrowService.GetCreateModelAsync();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BorrowCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // TODO:
            // Lấy PersonnelId từ User Login.
            // Tạm thời hard-code.
            var personnelId = int.Parse(
    User.FindFirst("PersonnelId")!.Value);

            await _borrowService.CreateAsync(
                model,
                personnelId);

            TempData["Success"] =
                "Tạo phiếu mượn thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(model);
        }
    }

    #endregion

    #region Detail

    [HttpGet]
    public async Task<IActionResult> Detail(
        int id)
    {
        var model =
            await _borrowService.GetDetailAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    #endregion

    #region Return Books

    [HttpGet]
    public async Task<IActionResult> ReturnBooks(
        int id)
    {
        var model =
            await _borrowService.GetDetailAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnBooks(
        int borrowRecordId,
        List<BorrowReturnItemViewModel> books)
    {
        try
        {
            await _borrowService.ReturnBooksAsync(
                borrowRecordId,
                books);

            TempData["Success"] =
                "Trả sách thành công.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.Message;

            return RedirectToAction(
                nameof(ReturnBooks),
                new
                {
                    id = borrowRecordId
                });
        }
    }

    #endregion

    #region Lookup

    [HttpGet]
    public async Task<IActionResult> SearchResidents(
        string? term)
    {
        var residents =
            await _borrowService
                .SearchResidentsAsync(term);

        return Json(
            residents.Select(x => new
            {
                id = x.ResidentId,

                text =
                    $"{x.FullName} - {x.Email} - {x.PhoneNumber}"
            }));
    }

    [HttpGet]
    public async Task<IActionResult> SearchBooks(
        string? term)
    {
        var books =
            await _borrowService
                .SearchBooksAsync(term);

        return Json(
            books.Select(x => new
            {
                id = x.BookId,

                text =
                    $"{x.Title} | {x.AuthorName} | {x.CategoryName} (Còn: {x.AvailableQuantity})"
            }));
    }

    #endregion
}