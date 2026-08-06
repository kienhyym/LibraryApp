namespace LibraryApp.ViewModels.Home;

public class HomeViewModel
{
    public string? Keyword { get; set; }

    public List<BookCardViewModel> NewBooks { get; set; }
        = new();

    public List<BookCardViewModel> PopularBooks { get; set; }
        = new();

    public List<CategoryCardViewModel> Categories { get; set; }
        = new();

    public int TotalBooks { get; set; }

    public int TotalAuthors { get; set; }

    public int TotalCategories { get; set; }
     public int TotalResidents { get; set; }

    public int TotalBorrowRecords { get; set; }
}