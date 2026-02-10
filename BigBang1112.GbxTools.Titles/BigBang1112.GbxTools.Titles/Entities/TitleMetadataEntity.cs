using System.ComponentModel.DataAnnotations;

namespace BigBang1112.GbxTools.Titles.Entities;

public class TitleMetadataEntity
{
    public int Id { get; set; }
    public required TitleEntity Title { get; set; }
    public required DateTimeOffset LastUpdate { get; set; }
    [StringLength(64)]
    public required string? ETag { get; set; }
    public required DateTimeOffset? LastModified { get; set; }
    public required long? ContentLength { get; set; }
    public required int Version { get; set; }
    public required int? AuthorVersion { get; set; }
    [StringLength(32)]
    public required string? AuthorLogin { get; set; }
    [StringLength(128)]
    public required string? AuthorNickname { get; set; }
    [StringLength(255)]
    public required string? AuthorZone { get; set; }
    [StringLength(128)]
    public required string? AuthorExtraInfo { get; set; }
    public required uint Flags { get; set; }
    public required uint GbxHeadersStart { get; set; }
    public required int? GbxHeadersSize { get; set; }
    public required int? GbxHeadersComprSize { get; set; }
    public required int? HeaderMaxSize { get; set; }
    public required uint? Size { get; set; }
    [MinLength(16), MaxLength(16)]
    public required byte[]? HeaderMD5 { get; set; }

    [MinLength(32), MaxLength(32)]
    public byte[]? Checksum { get; set; }
    public uint? HeaderFlags { get; set; }
    [StringLength(1024)]
    public string? Comments { get; set; }
    [StringLength(128)]
    public string? CreationBuildInfo { get; set; }
    [StringLength(255)]
    public string? AuthorUrl { get; set; }
    [StringLength(255)]
    public string? ManialinkUrl { get; set; }
    [StringLength(255)]
    public string? DownloadUrl { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    [StringLength(1024)]
    public string? Xml { get; set; }
    [StringLength(64)]
    public string? TitleId { get; set; }
    [StringLength(64)]
    public string? UsageSubDir { get; set; }
    [MinLength(16), MaxLength(16)]
    public byte[]? U01 { get; set; }

    public DateTimeOffset StoredAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<TitleFileEntity> Files { get; set; } = [];

    public ICollection<TitleIncludedPackEntity> IncludedPacks { get; set; } = [];

    public bool IsHeaderPrivate => (HeaderFlags & 1) != 0;

    public bool UseDefaultHeaderKey => (HeaderFlags & 2) != 0;

    public bool IsDataPrivate => (HeaderFlags & 4) != 0;

    public bool IsHeaderEncrypted
    {
        get
        {
            if (!IsHeaderPrivate)
            {
                return UseDefaultHeaderKey;
            }

            return true;
        }
    }
}
