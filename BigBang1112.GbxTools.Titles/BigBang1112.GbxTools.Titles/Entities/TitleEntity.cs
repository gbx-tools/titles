using System.ComponentModel.DataAnnotations;

namespace BigBang1112.GbxTools.Titles.Entities;

public class TitleEntity
{
    [StringLength(64)]
    public required string Id { get; set; }
    [StringLength(80)]
    public required string Name { get; set; }
    [StringLength(80)]
    public required string DeformattedName { get; set; }
    [StringLength(2048)]
    public required string Description { get; set; }
    [StringLength(255)]
    public required string Punchline { get; set; }
    [StringLength(32)]
    public required string AuthorLogin { get; set; }
    [StringLength(128)]
    public required string AuthorNickname { get; set; }
    [StringLength(160)]
    public required string DownloadUrl { get; set; }
    public required DateTimeOffset CreationDate { get; set; }
    public required DateTimeOffset LastUpdate { get; set; }
    public required int Cost { get; set; }
    public required int Registrations { get; set; }
    public required int PlayersLast24h { get; set; }
    public required int OnlinePlayers { get; set; }
    [StringLength(128)]
    public required string? FacebookUrl { get; set; }
    [StringLength(128)]
    public required string? TwitterUrl { get; set; }
    [StringLength(128)]
    public required string? YoutubeUrl { get; set; }
    [StringLength(128)]
    public required string? ForumUrl { get; set; }
    [StringLength(128)]
    public required string? WebsiteUrl { get; set; }
    [StringLength(160)]
    public required string CardUrl { get; set; }
    [StringLength(160)]
    public required string BackgroundUrl { get; set; }
    [StringLength(128)]
    public required string LogoUrl { get; set; }
    public required bool IsSolo { get; set; }
    public required bool IsMultiplayer { get; set; }
    public required bool IsEnvironment { get; set; }
    public required bool IsTrackmania { get; set; }
    public required bool IsShootmania { get; set; }
    public required bool IsMatchmaking { get; set; }
    [StringLength(6)]
    public required string PrimaryColor { get; set; }
    [StringLength(64)]
    public required string TitleMakerUid { get; set; }
    [StringLength(80)]
    public required string TitleMakerName { get; set; }
    [StringLength(128)]
    public required string TitlePageUrl { get; set; }

    public DateTimeOffset StoredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ArchivedAt { get; set; }
}
