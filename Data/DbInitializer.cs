using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Data;

public static class DbInitializer
{
    public static void Seed(LibDbContext context)
    {
        // Nếu đã có tài khoản thì không tạo nữa
        if (context.Taikhoans.Any())
        {
            return;
        }

        var admin = new Taikhoan
        {
            TenDangNhap = "admin",
            VaiTro = "Admin",
            TrangThai = true
        };

        var passwordHasher = new PasswordHasher<Taikhoan>();

        admin.MatKhau = passwordHasher.HashPassword(admin, "123456");

        context.Taikhoans.Add(admin);

        context.SaveChanges();
    }
}