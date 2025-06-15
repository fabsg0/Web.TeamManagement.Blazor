using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using fabsg0.Web.TeamManagement.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class MemberProvider(TeamManagementContext dbContext, MembershipProvider membershipProvider)
{
    public async Task<List<MemberModel>> GetMembers(int year, CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .AsNoTracking()
            .Include(x => x.MembershipFees)
            .Include(x => x.DepartmentMembers)
            .ThenInclude(x => x.Department)
            .Select(x => new MemberModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Sex = x.Sex,
                BirthDate = x.Birthdate,
                Email = x.Email,
                Telephone = x.Telephone,
                Street = x.Street,
                HouseNumber = x.HouseNumber,
                ZipCode = x.ZipCode,
                City = x.City,
                UpdatedAt = x.UpdatedAt,
                DepartmentMemberships = x.DepartmentMembers.ToList(),
                CurrentMembershipFee = x.MembershipFees
                    .FirstOrDefault(y => y.Year == year)!
            })
            .ToListAsync(cancellationToken);

        return members;
    }

    public async Task<Member> CreateMember(Member member, CancellationToken cancellationToken = default)
    {
        var memberId = Guid.NewGuid();
        member.Id = memberId;
        member.UpdatedAt = DateTime.UtcNow;

        await dbContext.Members.AddAsync(member, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await membershipProvider.CreateMembershipForYear(memberId,
            cancellationToken); // Creates membership entries for all years

        return member;
    }

    public async Task UpdateMember(MemberModel member, CancellationToken cancellationToken = default)
    {
        var memberToUpdate = await dbContext.Members
            .SingleOrDefaultAsync(x => x.Id == member.Id, cancellationToken);

        if (memberToUpdate == null) throw new Exception("Member not found.");

        memberToUpdate.FirstName = member.FirstName;
        memberToUpdate.LastName = member.LastName;
        memberToUpdate.Birthdate = member.BirthDate;
        memberToUpdate.Sex = member.Sex;
        memberToUpdate.Email = member.Email;
        memberToUpdate.Telephone = member.Telephone;
        memberToUpdate.Street = member.Street;
        memberToUpdate.HouseNumber = member.HouseNumber;
        memberToUpdate.ZipCode = member.ZipCode;
        memberToUpdate.City = member.City;
        memberToUpdate.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMember(Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members
            .Include(member => member.MembershipFees)
            .Include(member => member.DepartmentMembers)
            .SingleOrDefaultAsync(x => x.Id == memberId, cancellationToken);

        if (member == null) throw new Exception("Member not found.");

        if (member.MembershipFees.Count != 0) dbContext.MembershipFees.RemoveRange(member.MembershipFees);
        if (member.DepartmentMembers.Count != 0) dbContext.DepartmentMembers.RemoveRange(member.DepartmentMembers);

        dbContext.Members.Remove(member);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAllMembers(CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .Include(x => x.MembershipFees)
            .Include(x => x.DepartmentMembers)
            .ToListAsync(cancellationToken);

        foreach (var member in members)
        {
            if (member.MembershipFees.Count != 0) dbContext.MembershipFees.RemoveRange(member.MembershipFees);
            if (member.DepartmentMembers.Count != 0) dbContext.DepartmentMembers.RemoveRange(member.DepartmentMembers);
        }
        
        dbContext.Members.RemoveRange(members);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<MemberModel>> ExportMembers(CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .AsNoTracking()
            .Include(x => x.DepartmentMembers)                      // include join table
            .ThenInclude(dm => dm.Department)                   // include related department
            .Include(x => x.MembershipFees)                         // include all fees
            .ThenInclude(fee => fee.MemberhsipFeeDefinition)    // include fee definition
            .Select(x => new MemberModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Sex = x.Sex,
                BirthDate = x.Birthdate,
                Email = x.Email,
                Telephone = x.Telephone,
                Street = x.Street,
                HouseNumber = x.HouseNumber,
                ZipCode = x.ZipCode,
                City = x.City,
                UpdatedAt = x.UpdatedAt,
                DepartmentMemberships = x.DepartmentMembers.ToList(),
                CurrentMembershipFee = x.MembershipFees
                    .OrderByDescending(y => y.Year)
                    .FirstOrDefault()!
            })
            .ToListAsync(cancellationToken);

        return members;
    }
}