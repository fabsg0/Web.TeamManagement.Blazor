using fabsg0.Web.TeamManagement.Blazor.Components;
using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<TeamManagementContext>();

builder.Services.AddScoped<MemberProvider>();
builder.Services.AddScoped<MembershipProvider>();
builder.Services.AddScoped<DepartmentProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();