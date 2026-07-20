using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Nhanvien
{
    public int MaNhanVien { get; set; }

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? ChucVu { get; set; }

    public int? MaTaiKhoan { get; set; }

    public virtual Taikhoan? MaTaiKhoanNavigation { get; set; }

    public virtual ICollection<Phieumuon> Phieumuons { get; set; } = new List<Phieumuon>();
}
