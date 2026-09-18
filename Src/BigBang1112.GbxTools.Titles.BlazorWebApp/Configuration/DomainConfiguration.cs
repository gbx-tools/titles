using BigBang1112.GbxTools.Titles.Services;

namespace BigBang1112.GbxTools.Titles.BlazorWebApp.Configuration;

internal static class DomainConfiguration
{
    public static void AddDomainServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTitlesService();

        var archiveOptions = config.GetSection(TitleArchiveOptions.SectionName).Get<TitleArchiveOptions>() ?? new();
        services.AddSingleton(archiveOptions);
        services.AddSingleton<TitleArchiveStatus>();

        if (archiveOptions.Enabled)
        {
            services.AddHostedService<TitleArchiveBackgroundService>();
        }
    }
}
