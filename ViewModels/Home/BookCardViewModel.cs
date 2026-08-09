namespace LibraryApp.ViewModels.Home;

public class BookCardViewModel
{
    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? CoverImage { get; set; }

    public int AvailableQuantity { get; set; }

    public bool IsAvailable { get; set; }
    public bool IsFavorite { get; set; }
    
}