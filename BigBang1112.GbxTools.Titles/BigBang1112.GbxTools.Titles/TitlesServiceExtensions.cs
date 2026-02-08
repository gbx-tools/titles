using BigBang1112.GbxTools.Titles.Services;
using ManiaAPI.ManiaPlanetAPI.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BigBang1112.GbxTools.Titles;

public static class TitlesServiceExtensions
{
    public static void AddTitlesService(this IServiceCollection services)
    {
        services.AddManiaPlanetAPI();
        services.AddManiaPlanetIngameAPI();

        services.AddScoped<TitleService>();

        services.AddHybridCache();

        services.AddHostedService<TitleFetchHostedService>();
    }
}
