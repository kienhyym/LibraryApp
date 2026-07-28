using System;
using System.Collections.Generic;
using LibraryApp.Enums;

namespace LibraryApp.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public AccountRole AccountRole { get; set; }

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Personnel? Personnel { get; set; }

    public virtual Resident? Resident { get; set; }
}
