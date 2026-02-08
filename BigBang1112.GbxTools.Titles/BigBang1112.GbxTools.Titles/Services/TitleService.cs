using BigBang1112.GbxTools.Titles.Data;
using BigBang1112.GbxTools.Titles.Entities;
using BigBang1112.GbxTools.Titles.Models;
using ManiaAPI.ManiaPlanetAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace BigBang1112.GbxTools.Titles.Services;

public sealed class TitleService
{
    private readonly ManiaPlanetAPI mp;
    private readonly AppDbContext db;
    private readonly HybridCache cache;

    public TitleService(ManiaPlanetAPI mp, AppDbContext db, HybridCache cache)
    {
        this.mp = mp;
        this.db = db;
        this.cache = cache;
    }

    public async Task<IEnumerable<SearchTitleInfo>> SearchTitlesAsync(string query, CancellationToken cancellationToken = default)
    {
        var titles = await cache.GetOrCreateAsync("titles", async token =>
        {
            return await db.Titles
                .Select(x => new SearchTitleInfo
                {
                    Uid = x.Id,
                    Name = x.Name,
                    DeformattedName = x.DeformattedName,
                    CardUrl = x.CardUrl,
                    DownloadUrl = x.DownloadUrl
                })
                .ToListAsync(token);
        }, new() { Expiration = TimeSpan.FromMinutes(1) }, cancellationToken: cancellationToken);

        if (titles.Count == 0)
        {
            await cache.RemoveAsync("titles", cancellationToken);
        }

        return titles.Where(t => t.DeformattedName.Contains(query, StringComparison.OrdinalIgnoreCase));
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
}
