using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class Phieumuon
{
    public int MaPhieuMuon { get; set; }

    public int MaCuDan { get; set; }

    public int MaNhanVien { get; set; }

    public DateTime NgayMuon { get; set; }

    public DateTime HanTra { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<Chitietphieumuon> Chitietphieumuons { get; set; } = new List<Chitietphieumuon>();

    public virtual Cudan MaCuDanNavigation { get; set; } = null!;

    public virtual Nhanvien MaNhanVienNavigation { get; set; } = null!;
}
