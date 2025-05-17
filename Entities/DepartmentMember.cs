using System;
using System.Collections.Generic;

namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public partial class DepartmentMember
{
    public Guid Id { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? MemberId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Member? Member { get; set; }
}
