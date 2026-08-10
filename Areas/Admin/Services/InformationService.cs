using LibraryApp.Models;
using LibraryApp.Services.Interfaces;
using LibraryApp.ViewModels.Information;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Services;

public class InformationService : IInformationService
{
    private readonly LibDbContext _context;

    public InformationService(LibDbContext context)
    {
        _context = context;
    }

    public async Task<InformationViewModel?> GetInformationAsync(
        int accountId)
    {
        var personnel = await _context.Personnel
            .AsNoTracking()
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x =>
                x.AccountId == accountId);

        if (personnel == null)
        {
            return null;
        }

        return new InformationViewModel
        {
            PersonnelId = personnel.PersonnelId,

            FullName = personnel.FullName,

            Email = personnel.Account.Email,

            DateOfBirth = personnel.DateOfBirth,

            Gender = personnel.Gender,

            PhoneNumber = personnel.PhoneNumber,

            PersonnelAddress = personnel.PersonnelAddress,

            IsEmailVerified =
                personnel.Account.IsEmailVerified,

            CreatedAt =
                personnel.Account.CreatedAt
        };
    }

    public async Task<InformationUpdateViewModel?>
        GetInformationForUpdateAsync(
            int accountId)
    {
        var personnel = await _context.Personnel
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x =>
                x.AccountId == accountId);

        if (personnel == null)
        {
            return null;
        }

        return new InformationUpdateViewModel
        {
            FullName = personnel.FullName,

            DateOfBirth = personnel.DateOfBirth,

            Gender = personnel.Gender,

            PhoneNumber = personnel.PhoneNumber,

            PersonnelAddress = personnel.PersonnelAddress,

            Email = personnel.Account.Email,

        };
    }

    public async Task<(bool Success, string Message)>
        UpdateInformationAsync(
            int accountId,
            InformationUpdateViewModel model)
    {
        var personnel = await _context.Personnel
            .FirstOrDefaultAsync(x =>
                x.AccountId == accountId);

        if (personnel == null)
        {
            return (
                false,
                "Không tìm thấy thông tin nhân viên."
            );
        }

        personnel.FullName = model.FullName;

        personnel.DateOfBirth = model.DateOfBirth;

        personnel.Gender = model.Gender;

        personnel.PhoneNumber = model.PhoneNumber;

        personnel.PersonnelAddress =
            model.PersonnelAddress;

        await _context.SaveChangesAsync();

        return (
            true,
            "Cập nhật thông tin thành công."
        );
    }
}