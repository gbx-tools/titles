using System.ComponentModel.DataAnnotations;

namespace BigBang1112.GbxTools.Titles.Entities;

public class TitleIncludedPackEntity
{
    public int Id { get; set; }

    public required TitleMetadataEntity TitleMetadata { get; set; }

    [MinLength(32), MaxLength(32)]
    public required byte[] ContentsChecksum { get; set; }

    [StringLength(128)]
    public required string Name { get; set; }

    public required int AuthorVersion { get; set; }

    [StringLength(32)]
    public required string? AuthorLogin { get; set; }

    [StringLength(128)]
    public required string? AuthorNickname { get; set; }

    [StringLength(255)]
    public required string? AuthorZone { get; set; }

    [StringLength(128)]
    public required string? AuthorExtraInfo { get; set; }

    [StringLength(128)]
    public required string InfoManialinkUrl { get; set; }

    public required DateTimeOffset CreationDate { get; set; }

    public required uint IncludeDepth { get; set; }
}
