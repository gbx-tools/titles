using System.ComponentModel.DataAnnotations;

namespace BigBang1112.GbxTools.Titles.Entities;

public class TitleScriptEntity
{
    public int Id { get; set; }

    [Required]
    public TitleEntity Title { get; set; } = default!;
    public string TitleId { get; set; } = default!;

    [StringLength(64)]
    public required string FileName { get; set; }

    public required string MatchSettings { get; set; }
}
