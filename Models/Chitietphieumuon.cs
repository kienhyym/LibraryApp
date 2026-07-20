using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Chitietphieumuon
{
    public int MaCtpm { get; set; }

    public int MaPhieuMuon { get; set; }

    public int MaSach { get; set; }

    public int SoLuong { get; set; }

    public DateTime? NgayTraThucTe { get; set; }

    public string? TinhTrangTra { get; set; }

    public virtual Phieumuon MaPhieuMuonNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
