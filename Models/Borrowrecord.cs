using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Borrowrecord
{
    public int BorrowRecordId { get; set; }

    public int ResidentId { get; set; }

    public int PersonnelId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime DueDate { get; set; }

    public int BorrowRecordStatus { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Borrowrecorddetail> Borrowrecorddetails { get; set; } = new List<Borrowrecorddetail>();

    public virtual Personnel Personnel { get; set; } = null!;

    public virtual Resident Resident { get; set; } = null!;
}
