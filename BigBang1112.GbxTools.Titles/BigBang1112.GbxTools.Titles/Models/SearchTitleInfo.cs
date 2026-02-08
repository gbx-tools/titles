namespace BigBang1112.GbxTools.Titles.Models;

public sealed record SearchTitleInfo
{
    public required string Uid { get; init; }
    public required string Name { get; init; }
    public required string DeformattedName { get; init; }
    public required string CardUrl { get; init; }
    public required string DownloadUrl { get; init; }
}
