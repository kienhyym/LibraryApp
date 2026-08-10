using System;
using System.Collections.Generic;
using LibraryApp.Enums;

namespace LibraryApp.Models;

public partial class Personnel
{
    public int PersonnelId { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public Gender? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PersonnelAddress { get; set; }

    public string? Position { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Borrowrecord> Borrowrecords { get; set; } = new List<Borrowrecord>();
}
