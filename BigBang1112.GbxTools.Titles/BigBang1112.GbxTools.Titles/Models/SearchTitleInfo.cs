namespace BigBang1112.GbxTools.Titles.Models;

public sealed record SearchTitleInfo
{
    public required string Uid { get; init; }
    public required string Name { get; init; }
    public required string DeformattedName { get; init; }
    public required string CardUrl { get; init; }
    public required string DownloadUrl { get; init; }
    public required int Registrations { get; set; }
    public required int PlayersLast24h { get; set; }
    public required int OnlinePlayers { get; set; }
}
