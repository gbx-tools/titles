namespace BigBang1112.GbxTools.Titles.Services;

public sealed class TitleArchiveOptions
{
    public const string SectionName = "TitleArchive";

    public bool Enabled { get; set; }

    public string Path { get; set; } = "title-archive";

    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromDays(1);
}
