using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Favoritebook
{
    public int FavoriteBookId { get; set; }

    public int ResidentId { get; set; }

    public int BookId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Resident Resident { get; set; } = null!;
}
