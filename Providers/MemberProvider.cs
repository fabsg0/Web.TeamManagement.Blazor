using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using fabsg0.Web.TeamManagement.Blazor.Models;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class MemberProvider(TeamManagementContext dbContext, MembershipProvider membershipProvider)
{
    public async Task<List<MemberModel>> GetMembers(CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .AsNoTracking()
            .Include(x => x.MembershipFees)
            .Select(x => new MemberModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                BirthDate = x.Birthdate,
                Street = x.Street,
                HouseNumber = x.HouseNumber,
                ZipCode = x.ZipCode,
                City = x.City,
                UpdatedAt = x.UpdatedAt,
                DepartmentMemberships = x.DepartmentMembers.ToList(),
                CurrentMembershipFee = x.MembershipFees
                    .FirstOrDefault(y => y.Year == DateTime.Now.Year)
            })
            .ToListAsync(cancellationToken);

        return members;
    }

    public async Task CreateMember(Member member, CancellationToken cancellationToken = default)
    {
        var memberId = Guid.NewGuid();
        member.Id = memberId;
        
        await dbContext.Members.AddAsync(member, cancellationToken);
        await membershipProvider.CreateMembership(memberId, cancellationToken); // Creates a membership entry
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateMember(MemberModel member, CancellationToken cancellationToken = default)
    {
        var memberToUpdate = await dbContext.Members
            .SingleOrDefaultAsync(x => x.Id == member.Id, cancellationToken);

        if (memberToUpdate == null) throw new Exception("Member not found.");

        memberToUpdate.FirstName = member.FirstName;
        memberToUpdate.LastName = member.LastName;
        memberToUpdate.Birthdate = member.BirthDate;
        memberToUpdate.Street = member.Street;
        memberToUpdate.HouseNumber = member.HouseNumber;
        memberToUpdate.ZipCode = member.ZipCode;
        memberToUpdate.City = member.City;
        memberToUpdate.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMember(Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members
            .SingleOrDefaultAsync(x => x.Id == memberId, cancellationToken);

        if (member == null) throw new Exception("Member not found.");

        dbContext.Members.Remove(member);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}