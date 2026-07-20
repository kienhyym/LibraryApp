using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Models;

public partial class LibDbContext : DbContext
{
    public LibDbContext()
    {
    }

    public LibDbContext(DbContextOptions<LibDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chitietphieumuon> Chitietphieumuons { get; set; }

    public virtual DbSet<Cudan> Cudans { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Phieumuon> Phieumuons { get; set; }

    public virtual DbSet<Sach> Saches { get; set; }

    public virtual DbSet<Tacgium> Tacgia { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Theloai> Theloais { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=LibDB;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chitietphieumuon>(entity =>
        {
            entity.HasKey(e => e.MaCtpm).HasName("PK__CHITIETP__1E4E6072790A4602");

            entity.ToTable("CHITIETPHIEUMUON");

            entity.Property(e => e.MaCtpm).HasColumnName("MaCTPM");
            entity.Property(e => e.NgayTraThucTe).HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.TinhTrangTra).HasMaxLength(100);

            entity.HasOne(d => d.MaPhieuMuonNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.MaPhieuMuon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPM_PHIEUMUON");

            entity.HasOne(d => d.MaSachNavigation).WithMany(p => p.Chitietphieumuons)
                .HasForeignKey(d => d.MaSach)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPM_SACH");
        });

        modelBuilder.Entity<Cudan>(entity =>
        {
            entity.HasKey(e => e.MaCuDan).HasName("PK__CUDAN__080D9BC613B0563C");

            entity.ToTable("CUDAN");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__CUDAN__AD7C6528FDE719B5").IsUnique();

            entity.Property(e => e.DiaChiCanHo).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.Cudan)
                .HasForeignKey<Cudan>(d => d.MaTaiKhoan)
                .HasConstraintName("FK_CUDAN_TAIKHOAN");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NHANVIEN__77B2CA47F5E79D6F");

            entity.ToTable("NHANVIEN");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__NHANVIEN__AD7C6528C32FFA93").IsUnique();

            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.Nhanvien)
                .HasForeignKey<Nhanvien>(d => d.MaTaiKhoan)
                .HasConstraintName("FK_NHANVIEN_TAIKHOAN");
        });

        modelBuilder.Entity<Phieumuon>(entity =>
        {
            entity.HasKey(e => e.MaPhieuMuon).HasName("PK__PHIEUMUO__C4C8222230D1947B");

            entity.ToTable("PHIEUMUON");

            entity.Property(e => e.GhiChu).HasMaxLength(250);
            entity.Property(e => e.HanTra).HasColumnType("datetime");
            entity.Property(e => e.NgayMuon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Đang mượn");

            entity.HasOne(d => d.MaCuDanNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.MaCuDan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUMUON_CUDAN");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.Phieumuons)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHIEUMUON_NHANVIEN");
        });

        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasKey(e => e.MaSach).HasName("PK__SACH__B235742D0C2A68C6");

            entity.ToTable("SACH");

            entity.Property(e => e.AnhBia).HasMaxLength(255);
            entity.Property(e => e.NhaXuatBan).HasMaxLength(150);
            entity.Property(e => e.TenSach).HasMaxLength(200);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.ViTriKe).HasMaxLength(50);

            entity.HasOne(d => d.MaTacGiaNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTacGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SACH_TACGIA");

            entity.HasOne(d => d.MaTheLoaiNavigation).WithMany(p => p.Saches)
                .HasForeignKey(d => d.MaTheLoai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SACH_THELOAI");
        });

        modelBuilder.Entity<Tacgium>(entity =>
        {
            entity.HasKey(e => e.MaTacGia).HasName("PK__TACGIA__F24E6756DF7B4EB8");

            entity.ToTable("TACGIA");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.QuocTich).HasMaxLength(50);
            entity.Property(e => e.TenTacGia).HasMaxLength(100);
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TAIKHOAN__AD7C652956C5E59A");

            entity.ToTable("TAIKHOAN");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TAIKHOAN__55F68FC043686F04").IsUnique();

            entity.Property(e => e.MatKhau).HasMaxLength(500);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.VaiTro).HasMaxLength(20);
        });

        modelBuilder.Entity<Theloai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PK__THELOAI__D73FF34ADAAC7F4C");

            entity.ToTable("THELOAI");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenTheLoai).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
