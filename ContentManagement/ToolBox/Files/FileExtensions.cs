namespace ToolBox.Files;

public static class FileExtensions
{
    private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

    /// <summary>Compare two files for Equal Bytes</summary>
    /// <param name="file1">First File</param>
    /// <param name="file2">Second file to compare against</param>
    /// <param name="file1Bytes">(optional) Bytes of first file</param>
    /// <returns></returns>
    public static bool IsEqualBytes(this FileInfo file1, FileInfo file2, byte[]? file1Bytes = null, byte[]? file2Bytes = null)
    {
        if (!file1.Exists || !file2.Exists || file1.Length != file2.Length) return false;

        if (ReferenceEquals(file1Bytes, file2Bytes))
        {
            // same array was sent for both... they didn't actually read them each from the filesystem.
            // programming error.
            throw new ArgumentOutOfRangeException(nameof(file2Bytes), $"{nameof(file1Bytes)} is same reference.");
        }

        file1Bytes =
            file1Bytes != null && file1Bytes.Length == file1.Length ?
            file1Bytes :
            File.ReadAllBytes(file1.FullName);

        file2Bytes =
            file2Bytes != null && file2Bytes.Length == file2.Length ?
            file2Bytes :
            File.ReadAllBytes(file2.FullName);

        return file1Bytes.SequenceEqual(file2Bytes);
    }

    /// <summary>Look in a directory for a file matching another file.</summary>
    /// <returns>FileInfo of a matching file with the same bytes</returns>
    /// <param name="file">The file to find a match of</param>
    /// <param name="dir">Directory to look in for match. Defaults to same directory as the file.</param>
    /// <param name="searchPattern">Search Pattern, defaults to all files with same extension. Use "*" for any file</param>
    public static IEnumerable<FileInfo> FindMatchedFiles(this FileInfo file, DirectoryInfo? dir = null, string? searchPattern = null)
    {
        ArgumentNullException.ThrowIfNull(file);
        dir ??= file.Directory;
        searchPattern ??= file.Name;

        if (dir == null || file?.Directory == null || !dir.Exists || !file.Exists)
        {
            yield break;
        }

        Func<FileInfo, bool> predicate =
            dir.FullName.Equals(file.Directory.FullName, IgnoreCase) ?
            (FileInfo x) => x.Length == file.Length && !file.Name.Equals(x.Name, IgnoreCase) :
            (FileInfo x) => x.Length == file.Length;

        var matches = dir.EnumerateFiles(searchPattern).Where(predicate).ToArray();

        if (matches.Length == 0)
        {
            yield break;
        }

        var fileBytes = File.ReadAllBytes(file.FullName);
        foreach (var match in matches)
        {
            if (file.IsEqualBytes(match, fileBytes))
            {
                yield return match;
            }
        }
    }

}
