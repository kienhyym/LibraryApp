using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int CategoryId { get; set; }

    public int AuthorId { get; set; }

    public string? Publisher { get; set; }

    public int? PublicationYear { get; set; }

    public int Quantity { get; set; }

    public int AvailableQuantity { get; set; }

    public string? BookDescription { get; set; }

    public string? CoverImage { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Author Author { get; set; } = null!;

    public virtual ICollection<Borrowrecorddetail> Borrowrecorddetails { get; set; } = new List<Borrowrecorddetail>();

    public virtual Category Category { get; set; } = null!;
}
