using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Borrowrecorddetail
{
    public int BorrowRecordDetailId { get; set; }

    public int BorrowRecordId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string? ReturnCondition { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Borrowrecord BorrowRecord { get; set; } = null!;
}
