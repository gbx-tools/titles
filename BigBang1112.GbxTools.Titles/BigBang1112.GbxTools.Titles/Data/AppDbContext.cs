using BigBang1112.GbxTools.Titles.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigBang1112.GbxTools.Titles.Data;

public class AppDbContext : DbContext
{
    public DbSet<TitleEntity> Titles { get; set; }
    public DbSet<HistoricalPlayerCountEntity> HistoricalPlayerCounts { get; set; }
    public DbSet<HistoricalUpdateEntity> HistoricalUpdates { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
