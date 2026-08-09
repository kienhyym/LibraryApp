using LibraryApp.Common;
using LibraryApp.ViewModels.Home;

namespace LibraryApp.ViewModels.Favorite;

public class FavoriteViewModel
{
    public PaginatedList<BookCardViewModel> Books { get; set; } = default!;
}