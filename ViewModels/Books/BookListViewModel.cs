using LibraryApp.Common;
using LibraryApp.ViewModels.Home;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryApp.ViewModels.Book;

public class BookListViewModel
{
    public BookFilterViewModel Filter { get; set; }
        = new();

    public PaginatedList<BookCardViewModel> Books { get; set; }
        = null!;

    public List<SelectListItem> Categories { get; set; }
        = new();

    public List<SelectListItem> Authors { get; set; }
        = new();

    public int TotalBooks { get; set; }
}