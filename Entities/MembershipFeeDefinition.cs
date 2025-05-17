using System;
using System.Collections.Generic;

namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public partial class MembershipFeeDefinition
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Amount { get; set; }

    public int MinAge { get; set; }

    public int MaxAge { get; set; }

    public virtual ICollection<MembershipFee> MembershipFees { get; set; } = new List<MembershipFee>();
}
