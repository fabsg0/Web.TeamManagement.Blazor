namespace fabsg0.Web.TeamManagement.Blazor.Entities;

[NpgsqlTypes.PgName("sex_enum")]
public enum Sex
{
    [NpgsqlTypes.PgName("Mann")]
    Mann,
    [NpgsqlTypes.PgName("Frau")]
    Frau
}