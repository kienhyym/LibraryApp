using LibraryApp.Areas.Admin.ViewModels.Resident;
using LibraryApp.Common;
using LibraryApp.Enums;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class ResidentService : IResidentService
{
    private readonly LibDbContext _context;
    private readonly IOtpService _otpService;
    private readonly IEmailService _emailService;

    public ResidentService(
        LibDbContext context,
        IOtpService otpService,
        IEmailService emailService)
    {
        _context = context;
        _otpService = otpService;
        _emailService = emailService;
    }

    #region Query

    public async Task<PaginatedList<ResidentListViewModel>> GetPagedAsync(
        string? keyword,
        int page,
        int pageSize)
    {
        var query = _context.Residents
            .Include(x => x.Account)
            .OrderByDescending(x => x.ResidentId)
            .Select(x => new ResidentListViewModel
            {
                ResidentId = x.ResidentId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber,

                ApartmentNumber = x.ApartmentNumber,

                IsActive = x.Account.IsActive,

                CreatedAt = x.Account.CreatedAt
            });

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

        return await PaginatedList<ResidentListViewModel>
            .CreateAsync(query, page, pageSize);
    }

    public Task<ResidentCreateViewModel> GetCreateModelAsync()
    {
        return Task.FromResult(
            new ResidentCreateViewModel());
    }

    public async Task<ResidentEditViewModel?> GetEditModelAsync(int id)
    {
        return await _context.Residents
            .Include(x => x.Account)
            .Where(x => x.ResidentId == id)
            .Select(x => new ResidentEditViewModel
            {
                ResidentId = x.ResidentId,

                AccountId = x.AccountId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber,

                ApartmentNumber = x.ApartmentNumber,

                PermanentAddress = x.PermanentAddress,

                DateOfBirth = x.DateOfBirth,

                Gender = x.Gender,

                IsActive = x.Account.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ResidentDetailViewModel?> GetByIdAsync(int id)
    {
        return await _context.Residents
            .Include(x => x.Account)
            .Where(x => x.ResidentId == id)
            .Select(x => new ResidentDetailViewModel
            {
                ResidentId = x.ResidentId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber,

                ApartmentNumber = x.ApartmentNumber,

                PermanentAddress = x.PermanentAddress,

                DateOfBirth = x.DateOfBirth,

                Gender = x.Gender,

                IsActive = x.Account.IsActive,

                CreatedAt = x.Account.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    #endregion

    #region Validation

    public async Task<bool> ResidentExistsByEmailAsync(
        string email)
    {
        return await _context.Accounts
            .AnyAsync(x => x.Email == email);
    }

    public async Task<bool> ResidentExistsByPhoneAsync(
        string? phoneNumber)
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
            .FirstOrDefaultAsync(x => x.ResidentId == residentId);

        if (resident == null)
            return false;

        return await _context.Accounts
            .AnyAsync(x =>
                x.AccountId != resident.AccountId &&
                x.Email == email);
    }

    public async Task<bool> ResidentExistsByPhoneForUpdateAsync(
        string? phoneNumber,
        int residentId)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return await _context.Residents
            .AnyAsync(x =>
                x.ResidentId != residentId &&
                x.PhoneNumber == phoneNumber);
    }

    #endregion
    #region Registration

    public async Task SendOtpAsync(
    ResidentCreateViewModel model)
    {
        model.Email = model.Email.Trim();

        if (await ResidentExistsByEmailAsync(model.Email))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }
        if (await ResidentExistsByPhoneAsync(model.PhoneNumber))
        {
            throw new InvalidOperationException(
                "Số điện thoại đã tồn tại.");
        }

        var otp =
            await _otpService.GenerateOtpAsync(model.Email);

        await _emailService.SendOtpAsync(
            model.Email,
            otp);
    }

    public async Task ResendOtpAsync(string email)
    {
        email = email.Trim();

        var otp = await _otpService.GenerateOtpAsync(email);

        await _emailService.SendOtpAsync(
            email,
            otp);
    }

    public async Task VerifyOtpAndCreateAsync(
        ResidentCreateViewModel resident,
        ResidentVerifyOtpViewModel otp)
    {
        resident.Email = resident.Email.Trim();

        otp.Email = otp.Email.Trim();

        if (!resident.Email.Equals(
                otp.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Email xác thực không khớp.");
        }

        if (!await _otpService.VerifyOtpAsync(
                otp.Email,
                otp.OtpCode))
        {
            throw new InvalidOperationException(
                "Mã OTP không hợp lệ hoặc đã hết hạn.");
        }

        if (await ResidentExistsByEmailAsync(resident.Email))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var account = new Account
            {
                Email = resident.Email,

                AccountRole = AccountRole.Resident,

                PasswordHash = string.Empty,

                IsActive = true,

                IsEmailVerified = true,

                CreatedAt = DateTime.Now
            };

            var passwordHasher =
                new PasswordHasher<Account>();

            account.PasswordHash =
                passwordHasher.HashPassword(
                    account,
                    resident.Password);

            account.Resident = new Resident
            {
                FullName = resident.FullName.Trim(),

                DateOfBirth = resident.DateOfBirth,

                Gender = resident.Gender,

                PhoneNumber =
                    resident.PhoneNumber?.Trim(),

                ApartmentNumber =
                    resident.ApartmentNumber?.Trim(),

                PermanentAddress =
                    resident.PermanentAddress?.Trim()
            };

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            await _otpService.DeleteOtpAsync(
                resident.Email);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    #endregion
    #region CRUD

    public async Task UpdateAsync(
        ResidentEditViewModel model)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x =>
                x.ResidentId == model.ResidentId);

        if (resident == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy cư dân.");
        }

        if (await ResidentExistsByEmailForUpdateAsync(
                model.Email,
                model.ResidentId))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            if (await ResidentExistsByPhoneForUpdateAsync(
                    model.PhoneNumber,
                    model.ResidentId))
            {
                throw new InvalidOperationException(
                    "Số điện thoại đã tồn tại.");
            }
        }

        resident.FullName =
            model.FullName.Trim();

        resident.DateOfBirth =
            model.DateOfBirth;

        resident.Gender =
            model.Gender;

        resident.PhoneNumber =
            model.PhoneNumber?.Trim();

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

    public async Task ToggleActiveAsync(
        int residentId)
    {
        var resident = await _context.Residents
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x =>
                x.ResidentId == residentId);

        if (resident == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy cư dân.");
        }

        resident.Account.IsActive =
            !resident.Account.IsActive;

        await _context.SaveChangesAsync();
    }

    #endregion

}