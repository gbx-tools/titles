namespace BigBang1112.GbxTools.Titles.Services;

public sealed class TitleArchiveStatus
{
    private readonly object sync = new();
    private readonly bool enabled;
    private DateTimeOffset? lastStartedAt;
    private DateTimeOffset? lastCompletedAt;
    private string? lastError;
    private int archivedResources;
    private long archivedBytes;
    private int updatedResources;

    public TitleArchiveStatus(TitleArchiveOptions options)
    {
        enabled = options.Enabled;
    }

    public TitleArchiveSnapshot Snapshot()
    {
        lock (sync)
        {
            return new(enabled, lastStartedAt, lastCompletedAt, lastError, archivedResources, archivedBytes, updatedResources);
        }
    }

    internal void Started()
    {
        lock (sync)
        {
            lastStartedAt = DateTimeOffset.UtcNow;
            lastError = null;
        }
    }

    internal void Completed(int resourceCount, long totalBytes)
    {
        lock (sync)
        {
            archivedResources = resourceCount;
            archivedBytes = totalBytes;
            lastCompletedAt = DateTimeOffset.UtcNow;
        }
    }

    internal void ResourceUpdated()
    {
        lock (sync)
        {
            updatedResources++;
        }
    }

    internal void Failed(Exception exception)
    {
        lock (sync)
        {
            lastError = exception.Message;
        }
    }
}

public sealed record TitleArchiveSnapshot(
    bool Enabled,
    DateTimeOffset? LastStartedAt,
    DateTimeOffset? LastCompletedAt,
    string? LastError,
    int ArchivedResources,
    long ArchivedBytes,
    int UpdatedResources);
