using LibraryApp.Areas.Admin.ViewModels;
using LibraryApp.Common;
using LibraryApp.Enums;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LibraryApp.Services;

public class ResidentService : IResidentService
{
    private readonly LibDbContext _context;

    private readonly IOtpService _otpService;

    private readonly IWebHostEnvironment _environment;

    public ResidentService(
        LibDbContext context,
        IOtpService otpService,
        IWebHostEnvironment environment)
    {
        _context = context;
        _otpService = otpService;
        _environment = environment;
    }

    public async Task<PaginatedList<ResidentViewModel>> GetPagedAsync(
    string? keyword,
    int page,
    int pageSize)
    {
        var query =
            from resident in _context.Residents
            join account in _context.Accounts
                on resident.AccountId equals account.AccountId
            orderby resident.ResidentId descending
            select new ResidentViewModel
            {
                ResidentId = resident.ResidentId,
                AccountId = account.AccountId,

                FullName = resident.FullName,
                Email = account.Email,

                PhoneNumber = resident.PhoneNumber,
                ApartmentNumber = resident.ApartmentNumber,

                DateOfBirth = resident.DateOfBirth,
                Gender = resident.Gender,

                PermanentAddress = resident.PermanentAddress,

                IsActive = account.IsActive,
                IsEmailVerified = account.IsEmailVerified,

                CreatedAt = account.CreatedAt
            };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>

                x.FullName.Contains(keyword)

                || x.Email.Contains(keyword)

                || (x.PhoneNumber != null &&
                    x.PhoneNumber.Contains(keyword))

                || (x.ApartmentNumber != null &&
                    x.ApartmentNumber.Contains(keyword)));
        }

        return await PaginatedList<ResidentViewModel>
            .CreateAsync(query, page, pageSize);
    }
    public Task<ResidentViewModel> GetCreateModelAsync()
    {
        return Task.FromResult(new ResidentViewModel());
    }
    public async Task<ResidentViewModel?> GetEditModelAsync(int id)
    {
        return await (
            from resident in _context.Residents
            join account in _context.Accounts
                on resident.AccountId equals account.AccountId

            where resident.ResidentId == id

            select new ResidentViewModel
            {
                ResidentId = resident.ResidentId,

                AccountId = account.AccountId,

                FullName = resident.FullName,

                Email = account.Email,

                PhoneNumber = resident.PhoneNumber,

                ApartmentNumber = resident.ApartmentNumber,

                PermanentAddress = resident.PermanentAddress,

                DateOfBirth = resident.DateOfBirth,

                Gender = resident.Gender,

                IsActive = account.IsActive,

                IsEmailVerified = account.IsEmailVerified
            })
            .FirstOrDefaultAsync();
    }
    public async Task<ResidentViewModel?> GetByIdAsync(int id)
    {
        return await (
            from resident in _context.Residents
            join account in _context.Accounts
                on resident.AccountId equals account.AccountId

            where resident.ResidentId == id

            select new ResidentViewModel
            {
                ResidentId = resident.ResidentId,

                AccountId = account.AccountId,

                FullName = resident.FullName,

                Email = account.Email,

                PhoneNumber = resident.PhoneNumber,

                ApartmentNumber = resident.ApartmentNumber,

                PermanentAddress = resident.PermanentAddress,

                DateOfBirth = resident.DateOfBirth,

                Gender = resident.Gender,

                IsActive = account.IsActive,

                IsEmailVerified = account.IsEmailVerified,

                CreatedAt = account.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ResidentExistsByEmailAsync(string email)
    {
        return await _context.Accounts
            .AnyAsync(x => x.Email == email);
    }
    public async Task<bool> ResidentExistsByPhoneAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return await _context.Residents
            .AnyAsync(x => x.PhoneNumber == phoneNumber);
    }
    public async Task<bool> ResidentExistsByEmailForUpdateAsync(
        string email,
        int residentId)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.ResidentId == residentId);

        if (resident == null)
            return false;

        return await _context.Accounts
            .AnyAsync(x =>
                x.AccountId != resident.AccountId &&
                x.Email == email);
    }
    public async Task<bool> ResidentExistsByPhoneForUpdateAsync(
        string phoneNumber,
        int residentId)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return await _context.Residents
            .AnyAsync(x =>
                x.ResidentId != residentId &&
                x.PhoneNumber == phoneNumber);
    }

    public async Task SendOtpAsync(ResidentViewModel model)
    {
        if (await ResidentExistsByEmailAsync(model.Email))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            if (await ResidentExistsByPhoneAsync(model.PhoneNumber))
            {
                throw new InvalidOperationException(
                    "Số điện thoại đã tồn tại.");
            }
        }

        await _otpService.SendOtpAsync(model.Email);
    }
    public async Task ResendOtpAsync(string email)
    {
        await _otpService.SendOtpAsync(email);
    }
    public async Task<bool> IsOtpVerifiedAsync(string email)
    {
        return await _otpService.IsEmailVerifiedAsync(email);
    }
    public async Task VerifyOtpAndCreateAsync(
    ResidentViewModel model)
    {
        // 1. Kiểm tra OTP
        var verified = await _otpService.VerifyOtpAsync(
            model.Email,
            model.OtpCode!);

        if (!verified)
        {
            throw new InvalidOperationException(
                "Mã OTP không đúng hoặc đã hết hạn.");
        }

        // 2. Kiểm tra email đã được xác thực
        if (!await _otpService.IsEmailVerifiedAsync(model.Email))
        {
            throw new InvalidOperationException(
                "Email chưa được xác thực.");
        }

        // 3. Transaction
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Kiểm tra lại dữ liệu trước khi lưu
            if (await ResidentExistsByEmailAsync(model.Email))
            {
                throw new InvalidOperationException(
                    "Email đã tồn tại.");
            }

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                if (await ResidentExistsByPhoneAsync(model.PhoneNumber))
                {
                    throw new InvalidOperationException(
                        "Số điện thoại đã tồn tại.");
                }
            }

            // 4. Tạo Account
            var account = new Account
            {
                Email = model.Email.Trim(),

                AccountRole = AccountRole.Resident,

                IsActive = true,

                IsEmailVerified = true,

                CreatedAt = DateTime.Now
            };

            var passwordHasher =
                new PasswordHasher<Account>();

            account.PasswordHash =
                passwordHasher.HashPassword(
                    account,
                    model.Password);

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            // 5. Tạo Resident
            var resident = new Resident
            {
                AccountId = account.AccountId,

                FullName = model.FullName.Trim(),

                DateOfBirth = model.DateOfBirth,

                Gender = model.Gender,

                PhoneNumber = model.PhoneNumber?.Trim(),

                ApartmentNumber = model.ApartmentNumber?.Trim(),

                PermanentAddress = model.PermanentAddress?.Trim()
            };

            _context.Residents.Add(resident);

            await _context.SaveChangesAsync();

            // 6. Xóa OTP
            await _otpService.DeleteOtpAsync(model.Email);

            // 7. Commit
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }
    public async Task UpdateAsync(
    ResidentViewModel model)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x =>
                x.ResidentId == model.ResidentId);

        if (resident == null)
            throw new Exception("Không tìm thấy cư dân.");

        if (await ResidentExistsByEmailForUpdateAsync(
            model.Email,
            model.ResidentId))
        {
            throw new Exception("Email đã tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            if (await ResidentExistsByPhoneForUpdateAsync(
                model.PhoneNumber,
                model.ResidentId))
            {
                throw new Exception(
                    "Số điện thoại đã tồn tại.");
            }
        }

        resident.FullName = model.FullName.Trim();

        resident.DateOfBirth = model.DateOfBirth;

        resident.Gender = model.Gender;

        resident.PhoneNumber = model.PhoneNumber?.Trim();

        resident.ApartmentNumber =
            model.ApartmentNumber?.Trim();

        resident.PermanentAddress =
            model.PermanentAddress?.Trim();

        resident.Account.Email =
            model.Email.Trim();
        resident.Account.IsActive =
    model.IsActive;

        await _context.SaveChangesAsync();
    }
    public async Task ToggleActiveAsync(int residentId)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.ResidentId == residentId);

        if (resident == null)
            throw new Exception("Không tìm thấy cư dân.");

        resident.Account.IsActive = !resident.Account.IsActive;

        await _context.SaveChangesAsync();
    }
}