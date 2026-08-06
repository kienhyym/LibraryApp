namespace LibraryApp.ViewModels.Book;

public class BookDetailViewModel
{
    public int BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? CoverImage { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string? Publisher { get; set; }

    public int? PublishYear { get; set; }

    public int Quantity { get; set; }

    public int AvailableQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public string? Description { get; set; }

    public string? ShelfLocation { get; set; }
}