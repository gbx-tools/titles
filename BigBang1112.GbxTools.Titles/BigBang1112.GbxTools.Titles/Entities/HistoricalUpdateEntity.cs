namespace BigBang1112.GbxTools.Titles.Entities;

public class HistoricalUpdateEntity
{
    public int Id { get; set; }
    public required TitleEntity Title { get; set; }
    public required DateTimeOffset LastUpdate { get; set; }
}
