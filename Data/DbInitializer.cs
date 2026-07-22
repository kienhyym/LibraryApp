using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Data;

public static class DbInitializer
{
    public static void Seed(LibDbContext context)
    {
        // Nếu đã có tài khoản thì không tạo nữa
        if (context.Accounts.Any())
        {
            return;
        }

        var admin = new Account
        {
            Email = "admin@example.com",
            AccountRole = 1,
            IsActive = true
        };

        var passwordHasher = new PasswordHasher<Account>();

        admin.PasswordHash = passwordHasher.HashPassword(admin, "123456");

        context.Accounts.Add(admin);

        context.SaveChanges();
    }
}