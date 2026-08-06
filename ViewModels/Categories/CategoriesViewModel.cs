namespace LibraryApp.ViewModels.Category;

public class CategoriesViewModel
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int TotalBooks { get; set; }
}