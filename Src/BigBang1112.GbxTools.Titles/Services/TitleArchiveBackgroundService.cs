using BigBang1112.GbxTools.Titles.Data;
using BigBang1112.GbxTools.Titles.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BigBang1112.GbxTools.Titles.Services;

public sealed class TitleArchiveBackgroundService : BackgroundService
{
    private const string ManifestFileName = "archive-manifest.json";

    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IHostEnvironment environment;
    private readonly TitleArchiveOptions options;
    private readonly TitleArchiveStatus status;
    private readonly ILogger<TitleArchiveBackgroundService> logger;

    public TitleArchiveBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IHostEnvironment environment,
        TitleArchiveOptions options,
        TitleArchiveStatus status,
        ILogger<TitleArchiveBackgroundService> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.environment = environment;
        this.options = options;
        this.status = status;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.RefreshInterval <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("TitleArchive:RefreshInterval must be greater than zero.");
        }

        string archivePath;
        try
        {
            archivePath = Path.GetFullPath(options.Path, environment.ContentRootPath);
            Directory.CreateDirectory(archivePath);
        }
        catch (Exception exception)
        {
            status.Failed(exception);
            logger.LogError(exception, "Could not create the title archive directory.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ArchiveAsync(archivePath, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                status.Failed(exception);
                logger.LogError(exception, "Title archive synchronization failed.");
            }

            await Task.Delay(options.RefreshInterval, stoppingToken);
        }
    }

    private async Task ArchiveAsync(string archivePath, CancellationToken cancellationToken)
    {
        status.Started();
        var manifestPath = Path.Combine(archivePath, ManifestFileName);
        var manifest = await LoadManifestAsync(manifestPath, cancellationToken);

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var http = scope.ServiceProvider.GetRequiredService<HttpClient>();
        var titles = await db.Titles.AsNoTracking().ToListAsync(cancellationToken);
        var now = DateTimeOffset.UtcNow;

        foreach (var title in titles)
        {
            var titleDirectory = Path.Combine(archivePath, "titles", ToSafeDirectoryName(title.Id));

            await ArchiveResourceAsync(
                http,
                manifest,
                archivePath,
                Path.Combine(titleDirectory, "title.pak"),
                title.DownloadUrl,
                title.LastUpdate,
                forceRefresh: false,
                now,
                cancellationToken);

            await ArchiveResourceAsync(http, manifest, archivePath, Path.Combine(titleDirectory, "images", "card"), title.CardUrl, null, true, now, cancellationToken);
            await ArchiveResourceAsync(http, manifest, archivePath, Path.Combine(titleDirectory, "images", "background"), title.BackgroundUrl, null, true, now, cancellationToken);
            await ArchiveResourceAsync(http, manifest, archivePath, Path.Combine(titleDirectory, "images", "logo"), title.LogoUrl, null, true, now, cancellationToken);
        }

        await SaveManifestAsync(manifestPath, manifest, cancellationToken);
        status.Completed(manifest.Resources.Count, manifest.Resources.Values.Sum(x => x.Length));
    }

    private async Task ArchiveResourceAsync(
        HttpClient http,
        ArchiveManifest manifest,
        string archivePath,
        string destinationPath,
        string? url,
        DateTimeOffset? sourceUpdatedAt,
        bool forceRefresh,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return;
        }

        var key = Path.GetRelativePath(archivePath, destinationPath).Replace(Path.DirectorySeparatorChar, '/');
        manifest.Resources.TryGetValue(key, out var existing);
        var sourceChanged = sourceUpdatedAt.HasValue
            && (existing is null
                || !existing.SourceUpdatedAt.HasValue
                || sourceUpdatedAt.Value > existing.SourceUpdatedAt.Value);
        var refreshDue = forceRefresh && (existing is null || existing.LastCheckedAt <= now - options.RefreshInterval);

        if (existing is not null && existing.Url == uri.AbsoluteUri && !sourceChanged && !refreshDue && File.Exists(destinationPath))
        {
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (existing?.Url == uri.AbsoluteUri)
        {
            if (EntityTagHeaderValue.TryParse(existing.ETag, out var eTag))
            {
                request.Headers.IfNoneMatch.Add(eTag);
            }

            if (existing.LastModifiedAt.HasValue)
            {
                request.Headers.IfModifiedSince = existing.LastModifiedAt;
            }
        }

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotModified && existing is not null)
        {
            existing.LastCheckedAt = now;
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Could not archive {ResourceUrl}. Server returned {StatusCode}.", uri, response.StatusCode);
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
        var temporaryPath = destinationPath + ".downloading";

        try
        {
            await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var destination = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await source.CopyToAsync(destination, cancellationToken);
            await destination.FlushAsync(cancellationToken);
            File.Move(temporaryPath, destinationPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }

        manifest.Resources[key] = new ArchiveManifestEntry
        {
            Url = uri.AbsoluteUri,
            ETag = response.Headers.ETag?.ToString(),
            LastModifiedAt = response.Content.Headers.LastModified,
            LastCheckedAt = now,
            SourceUpdatedAt = sourceUpdatedAt,
            Length = new FileInfo(destinationPath).Length
        };
        status.ResourceUpdated();
    }

    private static async Task<ArchiveManifest> LoadManifestAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return new ArchiveManifest();
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<ArchiveManifest>(stream, cancellationToken: cancellationToken) ?? new ArchiveManifest();
    }

    private static async Task SaveManifestAsync(string path, ArchiveManifest manifest, CancellationToken cancellationToken)
    {
        var temporaryPath = path + ".saving";
        try
        {
            await using var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await JsonSerializer.SerializeAsync(stream, manifest, cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static string ToSafeDirectoryName(string titleId)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var directoryName = string.Concat(titleId.Select(character =>
            invalidCharacters.Contains(character)
            || character == Path.DirectorySeparatorChar
            || character == Path.AltDirectorySeparatorChar
            || character == '\\'
                ? '_'
                : character));

        return directoryName is "." or ".." ? "_" + directoryName : directoryName;
    }

    private sealed class ArchiveManifest
    {
        public ArchiveManifest()
        {
        }

        public Dictionary<string, ArchiveManifestEntry> Resources { get; set; } = new(StringComparer.Ordinal);
    }

    private sealed class ArchiveManifestEntry
    {
        public ArchiveManifestEntry()
        {
        }

        public string Url { get; set; } = string.Empty;
        public string? ETag { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public DateTimeOffset LastCheckedAt { get; set; }
        public DateTimeOffset? SourceUpdatedAt { get; set; }
        public long Length { get; set; }
    }
}
