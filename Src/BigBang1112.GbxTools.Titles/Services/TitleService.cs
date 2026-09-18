using BigBang1112.GbxTools.Titles.Data;
using BigBang1112.GbxTools.Titles.Entities;
using BigBang1112.GbxTools.Titles.Models;
using ManiaAPI.ManiaPlanetAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System.Collections.Immutable;
using System.Threading.RateLimiting;
using TmEssentials;

namespace BigBang1112.GbxTools.Titles.Services;

internal sealed class TitleService
{
    private readonly ManiaPlanetAPI mp;
    private readonly ManiaPlanetIngameAPI mpIngame;
    private readonly AppDbContext db;
    private readonly HybridCache cache;

    private static readonly FixedWindowRateLimiter TitleMissRateLimiter = new(new()
    {
        PermitLimit = 5,
        Window = TimeSpan.FromMinutes(1),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
    });

    public TitleService(ManiaPlanetAPI mp, ManiaPlanetIngameAPI mpIngame, AppDbContext db, HybridCache cache)
    {
        this.mp = mp;
        this.mpIngame = mpIngame;
        this.db = db;
        this.cache = cache;
    }

    public async Task<IEnumerable<SearchTitleInfo>> SearchTitlesAsync(string query, CancellationToken cancellationToken = default)
    {
        var titles = await cache.GetOrCreateAsync("titles", async token =>
        {
            return await db.Titles
                .OrderByDescending(x => x.OnlinePlayers)
                .Select(x => MapToSearchTitleInfo(x))
                .ToListAsync(token);
        }, new() { Expiration = TimeSpan.FromMinutes(1) }, cancellationToken: cancellationToken);

        if (titles.Count == 0)
        {
            await cache.RemoveAsync("titles", cancellationToken);
        }

        return titles.Where(t =>
            query.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .All(word => t.DeformattedName.Contains(word, StringComparison.OrdinalIgnoreCase) || t.Uid.Contains(word, StringComparison.OrdinalIgnoreCase)));
    }

    public async Task<TitleEntity?> GetTitleAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"title_{uid}", async token =>
        {
            return await db.Titles
                .Where(x => x.Id == uid)
                .AsNoTracking()
                .FirstOrDefaultAsync(token);
        }, new() { Expiration = TimeSpan.FromMinutes(1) }, cancellationToken: cancellationToken);
    }

    public async Task<TitleEntity?> FetchTitleAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"title_{uid}", async token =>
        {
            var title = await db.Titles
                .Where(x => x.Id == uid)
                .AsNoTracking()
                .FirstOrDefaultAsync(token);

            if (title is not null)
            {
                return title;
            }

            var lease = await TitleMissRateLimiter.AcquireAsync(cancellationToken: token);

            if (!lease.IsAcquired)
            {
                return null;
            }

            var ingameTitle = await mpIngame.GetTitleByUidAsync(uid, token);

            if (ingameTitle is null)
            {
                return null;
            }

            title = MapToTitleEntity(ingameTitle);

            await db.Titles.AddAsync(title, token);
            await db.SaveChangesAsync(token);
            await cache.RemoveAsync("titles", token);

            return title;
        }, new() { Expiration = TimeSpan.FromMinutes(1) }, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TitleScriptEntity>> FetchTitleScriptsAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"titlescripts_{uid}", async token =>
        {
            var title = await GetTitleAsync(uid, token);

            if (title is null)
            {
                return [];
            }

            ImmutableList<TitleScript> scripts;
            try // for private title packs this doesnt return proper status code
            {
                scripts = await mp.GetTitleScriptsAsync(uid, token);
            }
            catch
            {
                return [];
            }

            var existingScripts = await db.TitleScripts
                .Where(x => x.TitleId == uid)
                .ToListAsync(token);

            // if everything is equal, return existing scripts
            if (existingScripts.Count == scripts.Count
                && existingScripts.All(es => scripts.Any(s => s.FileName == es.FileName && s.MatchSettings == es.MatchSettings)))
            {
                return existingScripts;
            }

            var newScripts = scripts.Select(x => new TitleScriptEntity
            {
                TitleId = title.Id,
                FileName = x.FileName,
                MatchSettings = x.MatchSettings
            }).ToList();

            db.TitleScripts.RemoveRange(existingScripts);
            await db.TitleScripts.AddRangeAsync(newScripts, token);
            await db.SaveChangesAsync(token);

            return newScripts;
        }, new() { Expiration = TimeSpan.FromHours(1) }, cancellationToken: cancellationToken);
    }

    public async Task<TitleMetadataEntity?> GetTitleMetadataAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"titlemetadata_{uid}", async token =>
        {
            return await db.TitleMetadata
                .Where(x => x.Title.Id == uid)
                .OrderByDescending(x => x.LastUpdate)
                .AsNoTracking()
                .FirstOrDefaultAsync(token);
        }, new() { Expiration = TimeSpan.FromMinutes(1) }, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TitleIncludedPackEntity>> GetTitleIncludedPacksAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"titleincludedpacks_{uid}", async token =>
        {
            return await db.TitleIncludedPacks
                .Where(x => x.TitleMetadata.Title.Id == uid)
                .AsNoTracking()
                .ToListAsync(token);
        }, new() { Expiration = TimeSpan.FromHours(1) }, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<TitleFileEntity>> GetTitleFilesAsync(string uid, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync($"titlefiles_{uid}", async token =>
        {
            return await db.TitleFiles
                .Where(x => x.TitleMetadata.Title.Id == uid)
                .AsNoTracking()
                .ToListAsync(token);
        }, new() { Expiration = TimeSpan.FromHours(1) }, cancellationToken: cancellationToken);
    }

    public static SearchTitleInfo MapToSearchTitleInfo(TitleEntity t) => new()
    {
        Uid = t.Id,
        Name = t.Name,
        DeformattedName = t.DeformattedName,
        CardUrl = t.CardUrl,
        DownloadUrl = t.DownloadUrl,
        Registrations = t.Registrations,
        PlayersLast24h = t.PlayersLast24h,
        OnlinePlayers = t.OnlinePlayers
    };

    public static TitleEntity MapToTitleEntity(IngameTitle t) => new()
    {
        Id = t.Uid,
        Name = t.Name,
        DeformattedName = TextFormatter.Deformat(t.Name),
        Description = t.Description,
        Punchline = t.Punchline,
        AuthorLogin = t.AuthorLogin,
        AuthorNickname = t.AuthorNickname,
        DownloadUrl = t.DownloadUrl,
        CreationDate = t.CreationDate,
        LastUpdate = t.LastUpdate,
        Cost = t.Cost,
        Registrations = t.Registrations,
        PlayersLast24h = t.PlayersLast24h,
        OnlinePlayers = t.OnlinePlayers,
        FacebookUrl = t.FacebookUrl,
        TwitterUrl = t.TwitterUrl,
        YoutubeUrl = t.YoutubeUrl,
        ForumUrl = t.ForumUrl,
        WebsiteUrl = t.WebsiteUrl,
        CardUrl = t.CardUrl,
        BackgroundUrl = t.BackgroundUrl,
        LogoUrl = t.LogoUrl,
        IsSolo = t.IsSolo,
        IsMultiplayer = t.IsSolo,
        IsEnvironment = t.IsEnvironment,
        IsTrackmania = t.IsTrackmania,
        IsShootmania = t.IsShootmania,
        IsMatchmaking = t.IsMatchmaking,
        PrimaryColor = t.PrimaryColor,
        TitleMakerUid = t.TitleMakerUid,
        TitleMakerName = t.TitleMakerName,
        TitlePageUrl = t.TitlePageUrl
    };
}
