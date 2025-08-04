using Microsoft.Extensions.FileProviders;
using System.Collections;

namespace ToolBox.Compression.FileProviders;

internal class ZipDirectoryContents : IDirectoryContents
{
    public string? SubPath { get; init; }
    public bool Exists => Files.Count > 0;

    public List<ZipEntryFileInfo> Files { get; init; } = [];

    public IEnumerator<IFileInfo> GetEnumerator()
        => Files.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Files.GetEnumerator();
}
