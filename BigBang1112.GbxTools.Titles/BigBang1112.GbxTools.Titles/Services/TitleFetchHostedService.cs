using BigBang1112.GbxTools.Titles.Data;
using BigBang1112.GbxTools.Titles.Entities;
using ManiaAPI.ManiaPlanetAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using TmEssentials;

namespace BigBang1112.GbxTools.Titles.Services;

internal class TitleFetchHostedService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public TitleFetchHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var mpIngame = scope.ServiceProvider.GetRequiredService<ManiaPlanetIngameAPI>();
            var fetchedTitles = await mpIngame.SearchTitlesAsync(length: 1000, cancellationToken: stoppingToken);

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var storedTitles = await db.Titles.ToDictionaryAsync(t => t.Id, stoppingToken);

            foreach (var title in fetchedTitles)
            {
                if (!storedTitles.TryGetValue(title.Uid, out var storedTitle))
                {
                    await db.Titles.AddAsync(new TitleEntity
                    {
                        Id = title.Uid,
                        Name = title.Name,
                        DeformattedName = TextFormatter.Deformat(title.Name),
                        Description = title.Description,
                        Punchline = title.Punchline,
                        AuthorLogin = title.AuthorLogin,
                        AuthorNickname = title.AuthorNickname,
                        DownloadUrl = title.DownloadUrl,
                        CreationDate = title.CreationDate,
                        LastUpdate = title.LastUpdate,
                        Cost = title.Cost,
                        Registrations = title.Registrations,
                        PlayersLast24h = title.PlayersLast24h,
                        OnlinePlayers = title.OnlinePlayers,
                        FacebookUrl = title.FacebookUrl,
                        TwitterUrl = title.TwitterUrl,
                        YoutubeUrl = title.YoutubeUrl,
                        ForumUrl = title.ForumUrl,
                        WebsiteUrl = title.WebsiteUrl,
                        CardUrl = title.CardUrl,
                        BackgroundUrl = title.BackgroundUrl,
                        LogoUrl = title.LogoUrl,
                        IsSolo = title.IsSolo,
                        IsMultiplayer = title.IsSolo,
                        IsEnvironment = title.IsEnvironment,
                        IsTrackmania = title.IsTrackmania,
                        IsShootmania = title.IsShootmania,
                        IsMatchmaking = title.IsMatchmaking,
                        PrimaryColor = title.PrimaryColor,
                        TitleMakerUid = title.TitleMakerUid,
                        TitleMakerName = title.TitleMakerName,
                        TitlePageUrl = title.TitlePageUrl
                    }, stoppingToken);

                    continue;
                }

                if (storedTitle.LastUpdate != title.LastUpdate)
                {
                    await db.HistoricalUpdates.AddAsync(new()
                    {
                        Title = storedTitle,
                        LastUpdate = title.LastUpdate
                    }, stoppingToken);
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

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
