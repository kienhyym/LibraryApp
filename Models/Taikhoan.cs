using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Taikhoan
{
    public int MaTaiKhoan { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public bool TrangThai { get; set; }

    public virtual Cudan? Cudan { get; set; }

    public virtual Nhanvien? Nhanvien { get; set; }
}
