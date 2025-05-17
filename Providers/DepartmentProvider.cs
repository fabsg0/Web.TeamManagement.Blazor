using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using Microsoft.EntityFrameworkCore;

namespace fabsg0.Web.TeamManagement.Blazor.Providers;

public class DepartmentProvider(TeamManagementContext dbContext)
{
    public async Task CreateDepartment(Department department, CancellationToken cancellationToken = default)
    {
        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDepartment(Department department, CancellationToken cancellationToken = default)
    {
        var departmentToUpdate = await dbContext.Departments
            .SingleOrDefaultAsync(x => x.Id == department.Id, cancellationToken);
        
        if (departmentToUpdate == null) throw new Exception("Department not found.");
        
        departmentToUpdate.Name = department.Name;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteDepartment(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var department = await dbContext.Departments
            .SingleOrDefaultAsync(x => x.Id == departmentId, cancellationToken);

        if (department == null) throw new Exception("Department not found.");
        
        dbContext.Departments.Remove(department);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<Department>> GetDepartments(CancellationToken cancellationToken = default)
    {
        var departments = await dbContext.Departments
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return departments;
    }
    
    public async Task CreateDepartmentMembership(Guid memberId, Guid departmentId, CancellationToken cancellationToken = default)
    {
        var member = await dbContext.Members
            .Include(x => x.DepartmentMembers)
            .SingleOrDefaultAsync(x => x.Id == memberId, cancellationToken);

        if (member == null) throw new Exception("Member not found.");

        var departmentMembership = new DepartmentMember
        {
            MemberId = memberId,
            DepartmentId = departmentId
        };

        await dbContext.DepartmentMembers.AddAsync(departmentMembership, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}