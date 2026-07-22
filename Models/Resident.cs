using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Resident
{
    public int ResidentId { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public int? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? PermanentAddress { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Borrowrecord> Borrowrecords { get; set; } = new List<Borrowrecord>();
}
