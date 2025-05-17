using System;
using System.Collections.Generic;

namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public partial class Department
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DepartmentMember> DepartmentMembers { get; set; } = new List<DepartmentMember>();
}
