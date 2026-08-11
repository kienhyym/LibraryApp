using LibraryApp.Enums;
using LibraryApp.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Data;

public static class DbInitializer
{
    public static void Seed(LibDbContext context)
{
    if (context.Accounts.Any())
        return;

    var admin = new Account
    {
        Email = "admin@example.com",

        AccountRole = AccountRole.Admin,

        IsActive = true,

        IsEmailVerified = true,

        CreatedAt = DateTime.Now,

        Personnel = new Personnel
        {
            FullName = "Administrator",

            PhoneNumber = "0900000000",

            PersonnelAddress = "Library",

        }
    };

    var hasher = new PasswordHasher<Account>();

    admin.PasswordHash =
        hasher.HashPassword(admin, "123456");

    context.Accounts.Add(admin);

    context.SaveChanges();
}
}