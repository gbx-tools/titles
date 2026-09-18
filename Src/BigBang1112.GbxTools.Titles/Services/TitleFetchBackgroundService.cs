using BigBang1112.GbxTools.Titles.Data;
using ManiaAPI.ManiaPlanetAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using TmEssentials;

namespace BigBang1112.GbxTools.Titles.Services;

internal sealed class TitleFetchBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public TitleFetchBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mpIngame = scope.ServiceProvider.GetRequiredService<ManiaPlanetIngameAPI>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var exporter = scope.ServiceProvider.GetRequiredService<TitleExporter>();

            ImmutableList<IngameTitle> fetchedTitles;

            try
            { 
                fetchedTitles = await mpIngame.SearchTitlesAsync(length: 1000, cancellationToken: stoppingToken);
            }
            catch (Exception)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                continue;
            }

            var storedTitles = await db.Titles.ToDictionaryAsync(t => t.Id, stoppingToken);

            var titlesToExport = new List<IngameTitle>();

            foreach (var title in fetchedTitles)
            {
                if (!storedTitles.TryGetValue(title.Uid, out var storedTitle))
                {
                    titlesToExport.Add(title);
                    await db.Titles.AddAsync(TitleService.MapToTitleEntity(title), stoppingToken);
                    continue;
                }

                if (storedTitle.LastUpdate != title.LastUpdate)
                {
                    titlesToExport.Add(title);
                }

                if (storedTitle.Registrations != title.Registrations
                    || storedTitle.PlayersLast24h != title.PlayersLast24h
                    || storedTitle.OnlinePlayers != title.OnlinePlayers)
                {
                    await db.HistoricalPlayerCounts.AddAsync(new()
                    {
                        Title = storedTitle,
                        Registrations = title.Registrations,
                        PlayersLast24h = title.PlayersLast24h,
                        OnlinePlayers = title.OnlinePlayers
                    }, stoppingToken);
                }

                storedTitle.Name = title.Name;
                storedTitle.DeformattedName = TextFormatter.Deformat(title.Name);
                storedTitle.Description = title.Description;
                storedTitle.Punchline = title.Punchline;
                storedTitle.AuthorNickname = title.AuthorNickname;
                storedTitle.DownloadUrl = title.DownloadUrl;
                storedTitle.LastUpdate = title.LastUpdate;
                storedTitle.Cost = title.Cost;
                storedTitle.Registrations = title.Registrations;
                storedTitle.PlayersLast24h = title.PlayersLast24h;
                storedTitle.OnlinePlayers = title.OnlinePlayers;
                storedTitle.FacebookUrl = title.FacebookUrl;
                storedTitle.TwitterUrl = title.TwitterUrl;
                storedTitle.YoutubeUrl = title.YoutubeUrl;
                storedTitle.ForumUrl = title.ForumUrl;
                storedTitle.WebsiteUrl = title.WebsiteUrl;
                storedTitle.CardUrl = title.CardUrl;
                storedTitle.BackgroundUrl = title.BackgroundUrl;
                storedTitle.LogoUrl = title.LogoUrl;
                storedTitle.IsSolo = title.IsSolo;
                storedTitle.IsMultiplayer = title.IsMultiplayer;
                storedTitle.IsEnvironment = title.IsEnvironment;
                storedTitle.IsTrackmania = title.IsTrackmania;
                storedTitle.IsShootmania = title.IsShootmania;
                storedTitle.IsMatchmaking = title.IsMatchmaking;
                storedTitle.PrimaryColor = title.PrimaryColor;
                storedTitle.TitleMakerUid = title.TitleMakerUid;
                storedTitle.TitleMakerName = title.TitleMakerName;
                storedTitle.TitlePageUrl = title.TitlePageUrl;
            }

            await db.SaveChangesAsync(stoppingToken);

            foreach (var title in titlesToExport)
            {
                await exporter.EnqueueExportAsync(title, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
