using System.ComponentModel.DataAnnotations;

namespace BigBang1112.GbxTools.Titles.Entities;

public class TitleFileEntity
{
    public int Id { get; set; }
    public required TitleMetadataEntity TitleMetadata { get; set; }
    [StringLength(255)]
    public required string Name { get; set; }
    [StringLength(255)]
    public required string FolderPath { get; set; }
    public required uint ClassId { get; set; }
    public required uint Offset { get; set; }
    public required int UncompressedSize { get; set; }
    public required int CompressedSize { get; set; }
    public required int? Size { get; set; }
    [MinLength(16), MaxLength(16)]
    public required byte[]? Checksum { get; set; }
    public required ulong Flags { get; set; }

    public bool DontUseDummyWrite => (Flags & 0x100000000L) != 0;

    public bool PublicFile => (Flags & 0x2000000000000L) != 0;

    public bool ForceNoCrypt => (Flags & 0x4000000000000L) != 0;

    public bool IsCompressed => (Flags & 0x3C) != 0;

    public bool IsEncrypted
    {
        get
        {
            if (!ForceNoCrypt)
            {
                return !PublicFile;
            }

            return false;
        }
    }
}
