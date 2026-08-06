namespace LibraryApp.ViewModels.Book;

public class BookFilterViewModel
{
    public string? Keyword { get; set; }

    public int? CategoryId { get; set; }

    public int? AuthorId { get; set; }

    public string SortBy { get; set; } = "newest";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 12;
}