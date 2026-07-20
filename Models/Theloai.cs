using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Theloai
{
    public int MaTheLoai { get; set; }

    public string TenTheLoai { get; set; } = null!;

    public string? MoTa { get; set; }

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
