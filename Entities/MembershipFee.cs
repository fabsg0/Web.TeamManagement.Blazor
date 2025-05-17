using System;
using System.Collections.Generic;

namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public partial class MembershipFee
{
    public Guid Id { get; set; }

    public Guid? MemberhsipFeeDefinitionId { get; set; }

    public Guid? MemberId { get; set; }

    public bool? IsPaid { get; set; }

    public int Year { get; set; }

    public virtual Member? Member { get; set; }

    public virtual MembershipFeeDefinition? MemberhsipFeeDefinition { get; set; }
}
