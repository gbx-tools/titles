using BigBang1112.GbxTools.Titles.Entities;
using Reuniverse.Razor;

namespace BigBang1112.GbxTools.Titles.Models;

public static class TitleFileBrowserTree
{
    public static IReuFolder Create(IEnumerable<TitleFileEntity> files)
    {
        var root = new Folder(string.Empty);

        foreach (var file in files)
        {
            var folder = root;
            var segments = file.FolderPath
                .Split('\\', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Concat(file.Name.Split('\\', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToArray();

            if (segments.Length == 0)
            {
                continue;
            }

            foreach (var segment in segments[..^1])
            {
                folder = folder.GetOrAddFolder(segment);
            }

            folder.AddFile(new File(segments[^1], file.Size ?? file.UncompressedSize));
        }

        return root;
    }

    private sealed class Folder(string name) : IReuFolder
    {
        private readonly SortedDictionary<string, IReuEntry> entries = new(StringComparer.OrdinalIgnoreCase);

        public string Name { get; } = name;

        public IEnumerable<IReuEntry> Entries => entries.Values;

        public Folder GetOrAddFolder(string name)
        {
            if (entries.TryGetValue(name, out var entry) && entry is Folder folder)
            {
                return folder;
            }

            var newFolder = new Folder(name);
            entries[name] = newFolder;
            return newFolder;
        }

        public void AddFile(File file)
        {
            entries[file.Name] = file;
        }
    }

    private sealed class File(string name, long? size) : IReuFile
    {
        public string Name { get; } = name;

        public long? Size { get; } = size;

        public DateTimeOffset? LastModified => null;
    }
}
