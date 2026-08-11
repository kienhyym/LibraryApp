using System;
using System.Collections.Generic;
using LibraryApp.Enums;

namespace LibraryApp.Models;

public partial class Borrowrecorddetail
{
    public int BorrowRecordDetailId { get; set; }

    public int BorrowRecordId { get; set; }

    public int BookId { get; set; }

    public string? ReturnNote { get; set; }
    public DateTime? ReturnDate { get; set; }

    public ReturnStatus? ReturnStatus { get; set; }
    public decimal Penalty { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Borrowrecord BorrowRecord { get; set; } = null!;
}
