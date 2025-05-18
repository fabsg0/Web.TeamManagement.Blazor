using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class MembershipProvider(TeamManagementContext dbContext)
{
    public async Task CreateMembership(Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members
            .SingleOrDefaultAsync(x => x.Id == memberId, cancellationToken);

        if (member == null) throw new Exception("Member not found.");

        var memberAge = DateTime.Now.Year - member.Birthdate.Year;

        var membershipFeeDefinition = await dbContext.MembershipFeeDefinitions
            .SingleOrDefaultAsync(x => x.MinAge <= memberAge && x.MaxAge >= memberAge,
                cancellationToken);

        if (membershipFeeDefinition == null) throw new Exception("Membership fee definition not found.");

        var membership = new MembershipFee
        {
            MemberId = memberId,
            Year = DateTime.Now.Year,
            IsPaid = false,
            MemberhsipFeeDefinitionId = membershipFeeDefinition.Id
        };

        await dbContext.MembershipFees.AddAsync(membership, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
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

            var age = year - member.Birthdate.Year;
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
            
            var age = year - member.Birthdate.Year;
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