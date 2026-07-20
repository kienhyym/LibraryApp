using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Sach
{
    public int MaSach { get; set; }

    public string TenSach { get; set; } = null!;

    public int MaTheLoai { get; set; }

    public int MaTacGia { get; set; }

    public string? NhaXuatBan { get; set; }

    public int? NamXuatBan { get; set; }

    public int SoLuong { get; set; }

    public int SoLuongCon { get; set; }

    public string? ViTriKe { get; set; }

    public string? MoTa { get; set; }

    public string? AnhBia { get; set; }

    public bool TrangThai { get; set; }

    public virtual ICollection<Chitietphieumuon> Chitietphieumuons { get; set; } = new List<Chitietphieumuon>();

    public virtual Tacgium MaTacGiaNavigation { get; set; } = null!;

    public virtual Theloai MaTheLoaiNavigation { get; set; } = null!;
}
