namespace BigBang1112.GbxTools.Titles.Entities;

public class HistoricalPlayerCountEntity
{
    public int Id { get; set; }
    public required TitleEntity Title { get; set; }
    public required int Registrations { get; set; }
    public required int PlayersLast24h { get; set; }
    public required int OnlinePlayers { get; set; }
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
}
