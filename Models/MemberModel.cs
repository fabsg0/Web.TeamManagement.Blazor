using fabsg0.Web.TeamManagement.Blazor.Entities;

namespace fabsg0.Web.TeamManagement.Blazor.Models;

public class MemberModel
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public int? ZipCode { get; set; }
    public string? City { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<DepartmentMember>? DepartmentMemberships { get; set; }
    public MembershipFee? CurrentMembershipFee { get; set; }
}