namespace LibraryApp.ViewModels.Home;

public class CategoryCardViewModel
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int TotalBooks { get; set; }
}