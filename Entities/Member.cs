namespace fabsg0.Web.TeamManagement.Blazor.Entities;

public class Member
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Birthdate { get; set; }

    public string? Street { get; set; }

    public string? HouseNumber { get; set; }

    public int? ZipCode { get; set; }

    public string? City { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual ICollection<DepartmentMember> DepartmentMembers { get; set; } = new List<DepartmentMember>();

    public virtual ICollection<MembershipFee> MembershipFees { get; set; } = new List<MembershipFee>();
}