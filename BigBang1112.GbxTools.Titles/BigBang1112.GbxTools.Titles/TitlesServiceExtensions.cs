using BigBang1112.GbxTools.Titles.Services;
using ManiaAPI.ManiaPlanetAPI;
using ManiaAPI.ManiaPlanetAPI.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using System.Threading.RateLimiting;

namespace BigBang1112.GbxTools.Titles;

public static class TitlesServiceExtensions
{
    public static void AddTitlesService(this IServiceCollection services)
    {
        services.AddManiaPlanetAPI();
        services.AddManiaPlanetIngameAPI();

        services.AddScoped<TitleService>();

        services.AddHybridCache();

        services.AddHostedService<TitleFetchBackgroundService>();

        services.AddScoped<TitleExporter>();
        services.AddSingleton(_ => Channel.CreateUnbounded<IngameTitle>());
        services.AddHostedService<TitleExportBackgroundService>();
    }
}
