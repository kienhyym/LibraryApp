using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Tacgium
{
    public int MaTacGia { get; set; }

    public string TenTacGia { get; set; } = null!;

    public string? QuocTich { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
