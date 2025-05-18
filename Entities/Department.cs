namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public class Department
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public virtual ICollection<DepartmentMember> DepartmentMembers { get; set; } = new List<DepartmentMember>();
}