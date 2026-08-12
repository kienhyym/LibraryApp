using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

public class AuthorController : AdminBaseController
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    // Danh sách
   public async Task<IActionResult> Index(
    string? keyword,
    int page = 1)
{
    const int pageSize = 10;

    var model = await _authorService.GetPagedAsync(
        keyword,
        page,
        pageSize);

    ViewBag.Keyword = keyword;

    return View(model);
}

    // Form thêm
    public IActionResult Create()
    {
        return View();
    }

    // Lưu
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AuthorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _authorService.AuthorExistsByNameAsync(model.AuthorName))
        {
            ModelState.AddModelError(nameof(model.AuthorName),
                "Tên tác giả đã tồn tại.");

            return View(model);
        }

        await _authorService.CreateAsync(model);

        TempData["Success"] = "Thêm tác giả thành công.";

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _authorService.GetByIdAsync(id);

        if (author == null)
        {
            return NotFound();
        }

        return View(author);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AuthorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _authorService.AuthorExistsByNameForUpdateAsync(model.AuthorName, model.AuthorId))
        {
            ModelState.AddModelError(nameof(model.AuthorName),
                "Tên tác giả đã tồn tại.");

            return View(model);
        }

        await _authorService.UpdateAsync(model);

        TempData["Success"] = "Cập nhật tác giả thành công.";

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _authorService.DeleteAsync(id);

        TempData["Success"] = "Xóa tác giả thành công.";

        return RedirectToAction(nameof(Index));
    }


}