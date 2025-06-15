using ClosedXML.Excel;
using fabsg0.Web.TeamManagement.Blazor.Providers;
using Microsoft.AspNetCore.Mvc;

namespace fabsg0.Web.TeamManagement.Blazor.Middleware.Controllers;

[ApiController]
[Route("api/exportMembers")]
public class MemberExport(MemberProvider memberProvider, ILogger<MemberExport> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ExportMembersAsExcel(CancellationToken cancellationToken)
    {
        var members = await memberProvider.ExportMembers(cancellationToken);

        using var workBook = new XLWorkbook();
        var workSheet = workBook.Worksheets.Add("SU Raika Leisach - Members");

        workSheet.Cell(1, 1).Value = "First Name";
        workSheet.Cell(1,2).Value = "Last Name";
        workSheet.Cell(1,3).Value = "Sex";
        workSheet.Cell(1, 4).Value = "Email";
        workSheet.Cell(1, 5).Value = "Telephone";
        workSheet.Cell(1, 6).Value = "Street";
        workSheet.Cell(1, 7).Value = "House Number";
        workSheet.Cell(1, 8).Value = "Zip Code";
        workSheet.Cell(1, 9).Value = "City";
        workSheet.Cell(1, 10).Value = "Birth Date";
        workSheet.Cell(1, 11).Value = "Departments";
        workSheet.Cell(1, 12).Value = "Membership Fee";
        workSheet.Cell(1, 13).Value = "Membership Fee Paid";

        for (var i = 0; i < members.Count; i++)
        {
            var m = members[i];

            workSheet.Cell(i + 2, 1).Value = m.FirstName;
            workSheet.Cell(i + 2, 2).Value = m.LastName;
            workSheet.Cell(i + 2, 3).Value = m.Sex.ToString();
            workSheet.Cell(i + 2, 4).Value = m.Email;
            workSheet.Cell(i + 2, 5).Value = m.Telephone;
            workSheet.Cell(i + 2, 6).Value = m.Street;
            workSheet.Cell(i + 2, 7).Value = m.HouseNumber;
            workSheet.Cell(i + 2, 8).Value = m.ZipCode;
            workSheet.Cell(i + 2, 9).Value = m.City;
            workSheet.Cell(i + 2, 10).Value = m.BirthDate.ToString("dd.MM.yyyy");
            workSheet.Cell(i + 2, 11).Value = string.Join(", ",
                m.DepartmentMemberships?.Where(dm => dm.Department != null).Select(dm => dm.Department.Name) ?? []);
            workSheet.Cell(i + 2, 12).Value = m.CurrentMembershipFee.MemberhsipFeeDefinition?.Amount ?? 0;
            workSheet.Cell(i + 2, 13).Value = m.CurrentMembershipFee?.IsPaid == true ? "Yes" : "No";
        }

        using var stream = new MemoryStream();
        workBook.SaveAs(stream);
        stream.Position = 0;

        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Members_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }
}