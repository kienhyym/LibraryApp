using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class CategoryController : AdminBaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    #region Index

    public async Task<IActionResult> Index(
        string? keyword,
        int page = 1)
    {
        const int pageSize = 10;

        var model = await _categoryService.GetPagedAsync(
            keyword,
            page,
            pageSize);

        ViewBag.Keyword = keyword;

        return View(model);
    }

    #endregion

    #region Create

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _categoryService
            .CategoryNameExistsAsync(model.CategoryName))
        {
            ModelState.AddModelError(
                nameof(model.CategoryName),
                "Tên thể loại đã tồn tại.");

            return View(model);
        }

        await _categoryService.CreateAsync(model);

        TempData["Success"] =
            "Thêm thể loại thành công.";

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model =
            await _categoryService.GetByIdAsync(id);

        if (model == null)
            return NotFound();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        CategoryViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _categoryService
            .CategoryNameExistsForUpdateAsync(
                model.CategoryName,
                model.CategoryId))
        {
            ModelState.AddModelError(
                nameof(model.CategoryName),
                "Tên thể loại đã tồn tại.");

            return View(model);
        }

        await _categoryService.UpdateAsync(model);

        TempData["Success"] =
            "Cập nhật thể loại thành công.";

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
            await _categoryService.DeleteAsync(id);

            TempData["Success"] =
                "Xóa thể loại thành công.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion
}