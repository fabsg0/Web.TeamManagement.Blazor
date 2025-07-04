﻿using fabsg0.Web.TeamManagement.Blazor.Entities;

namespace fabsg0.Web.TeamManagement.Blazor.Models;

public class MemberModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Sex Sex { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public string? Street { get; set; }
    public string? HouseNumber { get; set; }
    public int? ZipCode { get; set; }
    public string? City { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<DepartmentMember>? DepartmentMemberships { get; set; }
    public MembershipFee CurrentMembershipFee { get; set; }

    public string PrimaryDepartmentName =>
        DepartmentMemberships?
            .Where(x => x.Department != null)
            .Select(x => x.Department!.Name)
            .OrderBy(name => name)
            .FirstOrDefault() ?? string.Empty;

}