using LibraryApp.Areas.Admin.ViewModels.Personnel;
using LibraryApp.Common;
using LibraryApp.Enums;
using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class PersonnelService : IPersonnelService
{
    private readonly LibDbContext _context;

    public PersonnelService(LibDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedList<PersonnelListViewModel>> GetPagedAsync(
    string? keyword,
    int page,
    int pageSize)
    {
        var query = _context.Personnel
            .Include(x => x.Account)
            .OrderByDescending(x => x.PersonnelId)
            .Select(x => new PersonnelListViewModel
            {
                PersonnelId = x.PersonnelId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber,

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

               );
        }

        return await PaginatedList<PersonnelListViewModel>
            .CreateAsync(query, page, pageSize);
    }
    public Task<PersonnelCreateViewModel> GetCreateModelAsync()
    {
        return Task.FromResult(
            new PersonnelCreateViewModel());
    }
    public async Task<PersonnelEditViewModel?> GetEditModelAsync(int id)
    {
        return await _context.Personnel
            .Include(x => x.Account)
            .Where(x => x.PersonnelId == id)
            .Select(x => new PersonnelEditViewModel
            {
                PersonnelId = x.PersonnelId,

                AccountId = x.AccountId,

                FullName = x.FullName,

                Email = x.Account.Email,

                PhoneNumber = x.PhoneNumber,

                PersonnelAddress = x.PersonnelAddress,

                DateOfBirth = x.DateOfBirth,

                Gender = x.Gender,

                IsActive = x.Account.IsActive
            })
            .FirstOrDefaultAsync();
    }
    public async Task<PersonnelDetailViewModel?> GetByIdAsync(int id)
{
    return await _context.Personnel
        .Include(x => x.Account)
        .Where(x => x.PersonnelId == id)
        .Select(x => new PersonnelDetailViewModel
        {
            PersonnelId = x.PersonnelId,

            FullName = x.FullName,

            Email = x.Account.Email,

            PhoneNumber = x.PhoneNumber,

            PersonnelAddress = x.PersonnelAddress,

            DateOfBirth = x.DateOfBirth,

            Gender = x.Gender,

            IsActive = x.Account.IsActive,

            CreatedAt = x.Account.CreatedAt
        })
        .FirstOrDefaultAsync();
}

    public async Task<bool> PersonnelExistsByEmailAsync(
        string email)
    {
        return await _context.Accounts
            .AnyAsync(x => x.Email == email);
    }
    public async Task<bool> PersonnelExistsByPhoneAsync(
        string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return await _context.Personnel
            .AnyAsync(x => x.PhoneNumber == phoneNumber);
    }

    public async Task<bool> PersonnelExistsByEmailForUpdateAsync(
        string email,
        int personnelId)
    {
        var personnel = await _context.Personnel
            .FirstOrDefaultAsync(x => x.PersonnelId == personnelId);

        if (personnel == null)
            return false;

        return await _context.Accounts
            .AnyAsync(x =>
                x.AccountId != personnel.AccountId &&
                x.Email == email);
    }
    public async Task<bool> PersonnelExistsByPhoneForUpdateAsync(
        string? phoneNumber,
        int personnelId)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        return await _context.Personnel
            .AnyAsync(x =>
                x.PersonnelId != personnelId &&
                x.PhoneNumber == phoneNumber);
    }

    public async Task CreateAsync(PersonnelCreateViewModel model)
    {
        if (await PersonnelExistsByEmailAsync(model.Email))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            if (await PersonnelExistsByPhoneAsync(model.PhoneNumber))
            {
                throw new InvalidOperationException(
                    "Số điện thoại đã tồn tại.");
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var account = new Account
            {
                Email = model.Email.Trim(),

                AccountRole = AccountRole.Personnel,

                IsActive = true,

                // Admin tạo nên mặc định xác thực
                IsEmailVerified = true,

                CreatedAt = DateTime.Now
            };

            var passwordHasher =
                new PasswordHasher<Account>();

            account.PasswordHash =
                passwordHasher.HashPassword(
                    account,
                    model.Password);

            account.Personnel = new Personnel
            {
                FullName = model.FullName.Trim(),

                DateOfBirth = model.DateOfBirth,

                Gender = model.Gender,

                PhoneNumber = model.PhoneNumber?.Trim(),

                PersonnelAddress =
                    model.PersonnelAddress?.Trim(),

            };

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public async Task UpdateAsync(PersonnelEditViewModel model)
    {
        var personnel = await _context.Personnel
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.PersonnelId == model.PersonnelId);

        if (personnel == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy nhân viên.");
        }

        if (await PersonnelExistsByEmailForUpdateAsync(
            model.Email,
            model.PersonnelId))
        {
            throw new InvalidOperationException(
                "Email đã tồn tại.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            if (await PersonnelExistsByPhoneForUpdateAsync(
                model.PhoneNumber,
                model.PersonnelId))
            {
                throw new InvalidOperationException(
                    "Số điện thoại đã tồn tại.");
            }
        }

        personnel.FullName = model.FullName.Trim();

        personnel.DateOfBirth = model.DateOfBirth;

        personnel.Gender = model.Gender;

        personnel.PhoneNumber = model.PhoneNumber?.Trim();

        personnel.PersonnelAddress =
            model.PersonnelAddress?.Trim();

        personnel.Account.Email =
            model.Email.Trim();

        personnel.Account.IsActive =
            model.IsActive;

        await _context.SaveChangesAsync();
    }

    public async Task ToggleActiveAsync(int personnelId)
    {
        var personnel = await _context.Personnel
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.PersonnelId == personnelId);

        if (personnel == null)
        {
            throw new InvalidOperationException(
                "Không tìm thấy nhân viên.");
        }

        personnel.Account.IsActive =
            !personnel.Account.IsActive;

        await _context.SaveChangesAsync();
    }
}