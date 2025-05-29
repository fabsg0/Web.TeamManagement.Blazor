using fabsg0.Web.TeamManagement.Blazor.Components;
using fabsg0.Web.TeamManagement.Blazor.Database;
using fabsg0.Web.TeamManagement.Blazor.Entities;
using fabsg0.Web.TeamManagement.Blazor.Providers;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Npgsql;

namespace fabsg0.Web.TeamManagement.Blazor;

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddMudServices();
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<Sex>();
        var dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<TeamManagementContext>(options =>
            options.UseNpgsql(dataSource));
        
        builder.Services.AddScoped<MemberProvider>();
        builder.Services.AddScoped<MembershipProvider>();
        builder.Services.AddScoped<DepartmentProvider>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();


        app.UseAntiforgery();

        app.UseStaticFiles();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}