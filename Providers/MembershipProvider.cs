using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class MembershipProvider(TeamManagementContext dbContext)
{
    public async Task CreateMembership(Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members
            .SingleOrDefaultAsync(cancellationToken);

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

    public async Task CreateMembershipsForAllForYear(int year, CancellationToken cancellationToken = default)
    {
        var members = await dbContext.Members
            .Include(x => x.MembershipFees)
            .ToListAsync(cancellationToken);

        foreach (var member in members)
        {
            // Check if a membership fee already exists for the given year
            if (member.MembershipFees.Any(x => x.Year == year)) continue;

            var membershipFeeDefinition = await dbContext.MembershipFeeDefinitions
                .SingleOrDefaultAsync(x => x.MinAge <= member.Birthdate.Year && x.MaxAge >= member.Birthdate.Year,
                    cancellationToken);

            if (membershipFeeDefinition == null) throw new Exception("Membership fee definition not found.");

            var membership = new MembershipFee
            {
                MemberId = member.Id,
                Year = year,
                IsPaid = false,
                MemberhsipFeeDefinitionId = membershipFeeDefinition.Id
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
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}