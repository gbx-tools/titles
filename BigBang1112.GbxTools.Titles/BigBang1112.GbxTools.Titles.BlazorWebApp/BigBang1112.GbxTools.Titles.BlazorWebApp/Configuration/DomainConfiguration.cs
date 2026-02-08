namespace BigBang1112.GbxTools.Titles.BlazorWebApp.Configuration;

internal static class DomainConfiguration
{
    public static void AddDomainServices(this IServiceCollection services)
    {
        services.AddTitlesService();
    }
}