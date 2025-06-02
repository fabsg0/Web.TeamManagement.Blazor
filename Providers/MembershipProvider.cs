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

    private async Task<bool> TryCreateMembershipFeeForYear(Member member, int year,
        List<MembershipFeeDefinition> feeDefinitions, CancellationToken cancellationToken)
    {
        var specialDefinitionId = Guid.Parse("292771e8-f698-4df0-9fb8-e728fda9239f");

        var feesForYear = member.MembershipFees
            .Where(f => f.Year == year)
            .ToList();

        var hasValidFee = feesForYear.Any(f => f.MemberhsipFeeDefinitionId != specialDefinitionId);
        if (hasValidFee) return false;

        var age = year - member.Birthdate.Year;
        if (age <= 0) return false;

        var feeDefinition = feeDefinitions
            .SingleOrDefault(x => x.MinAge <= age && x.MaxAge > age);

        if (feeDefinition == null) return false;

        // Add new valid membership
        var newMembership = new MembershipFee
        {
            MemberId = member.Id,
            Year = year,
            IsPaid = false,
            MemberhsipFeeDefinitionId = feeDefinition.Id,
            PaidAt = DateTime.MinValue
        };

        await dbContext.MembershipFees.AddAsync(newMembership, cancellationToken);

        // Remove any old special-fees for that year
        var specialFees = feesForYear
            .Where(f => f.MemberhsipFeeDefinitionId == specialDefinitionId)
            .ToList();

        if (specialFees.Count > 0)
            dbContext.MembershipFees.RemoveRange(specialFees);

        return true;
    }

    public async Task CreateMembershipsForYear(int year, CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .Include(x => x.MembershipFees)
            .ToListAsync(cancellationToken);

        var feeDefinitions = await dbContext.MembershipFeeDefinitions.ToListAsync(cancellationToken);

        foreach (var member in members)
        {
            await TryCreateMembershipFeeForYear(member, year, feeDefinitions, cancellationToken);
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
        var membershipYears = await dbContext.MembershipFees
            .Select(x => x.Year)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var year in membershipYears)
        {
            await TryCreateMembershipFeeForYear(member, year, feeDefinitions, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePaymentStatusForYear(Guid memberId, int year, CancellationToken cancellationToken = default)
    {
        var memberShip = await dbContext.MembershipFees
            .SingleOrDefaultAsync(x => x.MemberId == memberId && x.Year == year, cancellationToken);

        if (memberShip == null) throw new Exception("Membership fee not found.");

        memberShip.IsPaid = !memberShip.IsPaid;
        memberShip.PaidAt = memberShip.IsPaid ? DateTime.UtcNow : DateTime.MinValue;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}