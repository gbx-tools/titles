using BigBang1112.GbxTools.Titles.Data;
using BigBang1112.GbxTools.Titles.Entities;
using GBX.NET.PAK;
using ManiaAPI.ManiaPlanetAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using System.Threading.Channels;

namespace BigBang1112.GbxTools.Titles.Services;

internal sealed class TitleExportBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly Channel<IngameTitle> channel;

    public TitleExportBackgroundService(IServiceScopeFactory serviceScopeFactory, Channel<IngameTitle> channel)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ingameTitle = await channel.Reader.ReadAsync(stoppingToken);

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var http = scope.ServiceProvider.GetRequiredService<HttpClient>();

            using var response = await http.GetAsync(ingameTitle.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, stoppingToken);

            if (!response.IsSuccessStatusCode)
            {
                continue;
            }

            var titleEntity = await db.Titles.FirstOrDefaultAsync(x => x.Id == ingameTitle.Uid, stoppingToken);

            if (titleEntity is null)
            {
                continue;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(stoppingToken);
            await using var bufferedStream = new BufferedStream(stream);

            await using var pak = await Pak.ParseAsync(bufferedStream, cancellationToken: stoppingToken);

            var authorInfo = pak.AuthorInfo;

            var titleMetadataEntity = new TitleMetadataEntity
            {
                Title = titleEntity,
                LastUpdate = ingameTitle.LastUpdate,
                ETag = response.Headers.ETag?.ToString(),
                LastModified = response.Content.Headers.LastModified,
                ContentLength = response.Content.Headers.ContentLength,
                Version = pak.Version,
                AuthorVersion = authorInfo?.AuthorVersion,
                AuthorLogin = authorInfo?.AuthorLogin,
                AuthorNickname = authorInfo?.AuthorNickname,
                AuthorZone = authorInfo?.AuthorZone,
                AuthorExtraInfo = authorInfo?.AuthorExtraInfo,
                Flags = pak.Flags,
                GbxHeadersStart = pak.GbxHeadersStart,
                GbxHeadersSize = pak.GbxHeadersSize,
                GbxHeadersComprSize = pak.GbxHeadersComprSize,
                HeaderMaxSize = pak.HeaderMaxSize,
                Size = pak.Size,
                HeaderMD5 = pak.HeaderMD5
            };

            if (pak is Pak6 pak6)
            {
                titleMetadataEntity.Checksum = pak6.Checksum;
                titleMetadataEntity.HeaderFlags = pak6.HeaderFlags;
                titleMetadataEntity.Comments = pak6.Comments;
                titleMetadataEntity.CreationBuildInfo = pak6.CreationBuildInfo;
                titleMetadataEntity.AuthorUrl = pak6.AuthorUrl;
                titleMetadataEntity.ManialinkUrl = pak6.ManialinkUrl;
                titleMetadataEntity.DownloadUrl = pak6.DownloadUrl;
                titleMetadataEntity.CreationDate = pak6.CreationDate;
                titleMetadataEntity.Xml = pak6.Xml;
                titleMetadataEntity.TitleId = pak6.TitleId;
                titleMetadataEntity.UsageSubDir = pak6.UsageSubDir;
                titleMetadataEntity.U01 = pak6.U01;

                await db.TitleIncludedPacks.AddRangeAsync(EnumerateIncludedPackEntities(pak6, titleMetadataEntity), stoppingToken);
            }

            await db.TitleFiles.AddRangeAsync(EnumerateFileEntities(pak, titleMetadataEntity), stoppingToken);

            await db.TitleMetadata.AddAsync(titleMetadataEntity, stoppingToken);

            await db.SaveChangesAsync(stoppingToken);

            // if set as archived, also download the pak
        }
    }

    private static IEnumerable<TitleFileEntity> EnumerateFileEntities(Pak pak, TitleMetadataEntity titleMetadataEntity)
    {
        foreach (var (fullPath, file) in pak.Files)
        {
            var checksum = file.Checksum;
            var checksumValue = default(UInt128);

            if (checksum.HasValue)
            {
                checksumValue = checksum.Value;
            }

            yield return new TitleFileEntity
            {
                TitleMetadata = titleMetadataEntity,
                Name = file.Name,
                FolderPath = file.FolderPath,
                ClassId = file.ClassId,
                Offset = file.Offset,
                UncompressedSize = file.UncompressedSize,
                CompressedSize = file.CompressedSize,
                Size = file.Size,
                Checksum = checksum.HasValue ? MemoryMarshal.Cast<UInt128, byte>(MemoryMarshal.CreateReadOnlySpan(ref checksumValue, 1)).ToArray() : null,
                Flags = file.Flags
            };
        }
    }

    private static IEnumerable<TitleIncludedPackEntity> EnumerateIncludedPackEntities(Pak6 pak, TitleMetadataEntity titleMetadataEntity)
    {
        foreach (var includedPack in pak.IncludedPacks)
        {
            yield return new TitleIncludedPackEntity
            {
                TitleMetadata = titleMetadataEntity,
                ContentsChecksum = includedPack.ContentsChecksum,
                Name = includedPack.Name,
                AuthorVersion = includedPack.AuthorVersion,
                AuthorLogin = includedPack.AuthorLogin,
                AuthorNickname = includedPack.AuthorNickName,
                AuthorZone = includedPack.AuthorZone,
                AuthorExtraInfo = includedPack.AuthorExtraInfo,
                InfoManialinkUrl = includedPack.InfoManialinkUrl,
                CreationDate = includedPack.CreationDate,
                IncludeDepth = includedPack.IncludeDepth
            };
        }
    }
}
