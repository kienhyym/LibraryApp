using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class ProfileService : IProfileService
{
    private readonly LibDbContext _context;
    private readonly PasswordHasher<Account> _passwordHasher;
    public ProfileService(
        LibDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<Account>();
    }

    public async Task<ProfileViewModel?> GetProfileAsync(
        int accountId)
    {
        var resident = await _context.Residents

            .AsNoTracking()

            .Include(x => x.Account)

            .FirstOrDefaultAsync(x =>
                x.AccountId == accountId);

        if (resident == null)
        {
            return null;
        }

        return new ProfileViewModel
        {
            ResidentId = resident.ResidentId,

            FullName = resident.FullName,

            Email = resident.Account.Email,

            DateOfBirth = resident.DateOfBirth,

            Gender = resident.Gender,

            PhoneNumber = resident.PhoneNumber,

            ApartmentNumber = resident.ApartmentNumber,

            PermanentAddress = resident.PermanentAddress,

            IsEmailVerified =
                resident.Account.IsEmailVerified,

            CreatedAt =
                resident.Account.CreatedAt,

            // Tạm thời

            TotalBorrowedBooks =
    await _context.Borrowrecorddetails
        .CountAsync(x =>
            x.BorrowRecord.ResidentId ==
            resident.ResidentId),

            BorrowingBooks =
    await _context.Borrowrecorddetails
        .CountAsync(x =>
            x.BorrowRecord.ResidentId ==
            resident.ResidentId &&
            x.ReturnDate == null),

            OverdueBooks =
    await _context.Borrowrecorddetails
        .CountAsync(x =>
            x.BorrowRecord.ResidentId ==
            resident.ResidentId &&
            x.ReturnDate == null &&
            x.BorrowRecord.DueDate < DateTime.Now)
        };
    }

    public async Task<ProfileUpdateViewModel?> GetProfileForUpdateAsync(
    int accountId)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (resident == null)
        {
            return null;
        }

        return new ProfileUpdateViewModel
        {
            FullName = resident.FullName,

            DateOfBirth = resident.DateOfBirth,

            Gender = resident.Gender,

            PhoneNumber = resident.PhoneNumber,

            ApartmentNumber = resident.ApartmentNumber,

            PermanentAddress = resident.PermanentAddress,

            Email = resident.Account.Email
        };
    }
    public async Task<(bool Success, string Message)> UpdateProfileAsync(
        int accountId,
        ProfileUpdateViewModel model)
    {
        var resident = await _context.Residents
            .FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (resident == null)
        {
            return (false, "Không tìm thấy thông tin cư dân.");
        }

        resident.FullName = model.FullName;

        resident.DateOfBirth = model.DateOfBirth;

        resident.Gender = model.Gender;

        resident.PhoneNumber = model.PhoneNumber;

        resident.ApartmentNumber = model.ApartmentNumber;

        resident.PermanentAddress = model.PermanentAddress;

        await _context.SaveChangesAsync();

        return (true, "Cập nhật hồ sơ thành công.");
    }
    public async Task<(bool Success, string Message)> ChangePasswordAsync(
    int accountId,
    ChangePasswordViewModel model)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (account == null)
        {
            return (false, "Không tìm thấy tài khoản.");
        }
        // Không cho phép khoảng trắng đầu/cuối
        if (model.NewPassword != model.NewPassword.Trim())
        {
            return (false,
                "Mật khẩu mới không được chứa khoảng trắng ở đầu hoặc cuối.");
        }
        var verifyResult =
            _passwordHasher.VerifyHashedPassword(
                account,
                account.PasswordHash,
                model.CurrentPassword);

        if (verifyResult == PasswordVerificationResult.Failed)
        {
            return (false, "Mật khẩu hiện tại không đúng.");
        }

        // Không cho phép trùng mật khẩu cũ
        if (model.CurrentPassword == model.NewPassword)
        {
            return (false,
                "Mật khẩu mới phải khác mật khẩu hiện tại.");
        }

        account.PasswordHash =
            _passwordHasher.HashPassword(
                account,
                model.NewPassword);

        await _context.SaveChangesAsync();

        return (true, "Đổi mật khẩu thành công.");
    }
}