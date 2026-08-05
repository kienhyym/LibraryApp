namespace LibraryApp.Areas.Admin.ViewModels.Borrow;

public class BookLookupViewModel
{
    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int AvailableQuantity { get; set; }

    public string DisplayText =>
        $"{Title} - {AuthorName} - {CategoryName}";
}