using ManiaAPI.ManiaPlanetAPI;
using System.Threading.Channels;

namespace BigBang1112.GbxTools.Titles.Services;

internal sealed class TitleExporter
{
    private readonly Channel<IngameTitle> channel;

    public TitleExporter(Channel<IngameTitle> channel)
    {
        this.channel = channel;
    }

    public async Task EnqueueExportAsync(IngameTitle title, CancellationToken cancellationToken = default)
    {
        await channel.Writer.WriteAsync(title, cancellationToken);
    }
}
