using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class MembershipProvider(TeamManagementContext dbContext)
{
    public async Task<List<int>> GetMembershipYears(CancellationToken cancellationToken = default)
    {
        var years = await dbContext.MembershipFees
            .Select(x => x.Year)
            .Distinct()
            .ToListAsync(cancellationToken);

        return years;
    }

    public async Task CreateMembershipsForYear(int year, CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .Include(x => x.MembershipFees)
            .ToListAsync(cancellationToken);

        var feeDefinitions = await dbContext.MembershipFeeDefinitions.ToListAsync(cancellationToken);

        foreach (var member in members)
        {
            if (member.MembershipFees.Any(x => x.Year == year)) continue;

            if (member.Birthdate == default) continue;
            var age = CalculateAge(member.Birthdate, year);
            var feeDefinition = feeDefinitions.SingleOrDefault(x => x.MinAge <= age && x.MaxAge >= age);

            if (feeDefinition == null)
                throw new Exception($"Membership fee definition not found for member {member.Id} with age {age}.");

            var membership = new MembershipFee
            {
                MemberId = member.Id,
                Year = year,
                IsPaid = false,
                MemberhsipFeeDefinitionId = feeDefinition.Id,
                PaidAt = DateTimeOffset.MinValue
            };

            await dbContext.MembershipFees.AddAsync(membership, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateMembershipForYear(Guid memberId, CancellationToken cancellationToken)
    {
        var member = await dbContext.Members
            .Include(x => x.MembershipFees)
            .SingleOrDefaultAsync(x => x.Id == memberId, cancellationToken);

        if (member == null) throw new Exception("Member not found.");

        var feeDefinitions = await dbContext.MembershipFeeDefinitions.ToListAsync(cancellationToken);
        var membershipYears = await dbContext.MembershipFees.Select(x => x.Year).ToListAsync(cancellationToken);

        foreach (var year in membershipYears)
        {
            if (member.MembershipFees.Any(x => x.Year == year)) continue;

            if (member.Birthdate == default) continue;
            var age = CalculateAge(member.Birthdate, year);
            var feeDefinition = feeDefinitions.SingleOrDefault(x => x.MinAge <= age && x.MaxAge >= age);

            if (feeDefinition == null)
                throw new Exception($"Membership fee definition not found for member {member.Id} with age {age}.");

            var membership = new MembershipFee
            {
                MemberId = member.Id,
                Year = year,
                IsPaid = false,
                MemberhsipFeeDefinitionId = feeDefinition.Id,
                PaidAt = DateTimeOffset.MinValue
            };

            await dbContext.MembershipFees.AddAsync(membership, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static int CalculateAge(DateOnly birthdate, int year)
    {
        var referenceDate = new DateOnly(year, 1, 1);
        var age = referenceDate.Year - birthdate.Year;
        if (birthdate > referenceDate.AddYears(-age)) age--;
        return age;
    }

    
    public async Task ChangePaymentStatusForYear(Guid memberId, int year, CancellationToken cancellationToken = default)
    {
        var memberShip = await dbContext.MembershipFees
            .SingleOrDefaultAsync(x => x.MemberId == memberId && x.Year == year, cancellationToken);

        if (memberShip == null) throw new Exception("Membership fee not found.");

        memberShip.IsPaid = !memberShip.IsPaid;
        memberShip.PaidAt = memberShip.IsPaid ? DateTimeOffset.Now : DateTimeOffset.MinValue;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}