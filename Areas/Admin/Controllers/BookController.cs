using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class BookController : AdminBaseController
{
    private readonly IBookService _bookService;

    public BookController(
        IBookService bookService)
    {
        _bookService = bookService;
    }

    #region Index

    public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
    {
        const int pageSize = 10;

        var model = await _bookService.GetPagedAsync(
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
        var model = await _bookService
            .GetCreateModelAsync();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model = await _bookService
                .GetCreateModelAsync();

            return View(model);
        }

        if (await _bookService
            .BookExistsByTitleAsync(model.Title))
        {
            ModelState.AddModelError(
                nameof(model.Title),
                "Tên sách đã tồn tại.");

            await _bookService.LoadDropdownDataAsync(model);

            return View(model);
        }

        await _bookService.CreateAsync(model);

        TempData["Success"] =
            "Thêm sách thành công.";

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _bookService
            .GetEditModelAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await _bookService.LoadDropdownDataAsync(model);

            return View(model);
        }

        if (await _bookService
            .BookExistsByTitleForUpdateAsync(
                model.Title,
                model.BookId))
        {
            ModelState.AddModelError(
                nameof(model.Title),
                "Tên sách đã tồn tại.");

            await _bookService.LoadDropdownDataAsync(model);

            return View(model);
        }

        await _bookService.UpdateAsync(model);

        TempData["Success"] =
            "Cập nhật sách thành công.";

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Delete

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _bookService.DeleteAsync(id);

            TempData["Success"] =
                "Xóa sách thành công.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}