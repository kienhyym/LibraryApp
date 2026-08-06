namespace LibraryApp.ViewModels.Author;

public class AuthorsViewModel
{
    public int AuthorId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string? Nationality { get; set; }

    public string? Notes { get; set; }

    public int TotalBooks { get; set; }
}