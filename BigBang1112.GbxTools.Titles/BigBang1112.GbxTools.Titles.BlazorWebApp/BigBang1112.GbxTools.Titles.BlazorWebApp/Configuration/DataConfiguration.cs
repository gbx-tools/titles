using BigBang1112.GbxTools.Titles.Data;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.GbxTools.Titles.BlazorWebApp.Configuration;

internal static class DataConfiguration
{
    public static void AddDataServices(this IServiceCollection services, IConfiguration config, IHostEnvironment environment)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionStr = config.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr), options =>
            {
                options.MigrationsAssembly(typeof(DataConfiguration).Assembly);
            });
        });
    }

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}
