using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;

namespace ToolBox.Compression;

public static class ZipExtensions
{
    private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
    private const int NotFoundIndex = -1;

    public static List<FileInfo> ZipDirectory(this DirectoryInfo dir, Func<DirectoryInfo, string?>? passwordLookup = null, bool recurse = false, Action<string>? writeLine = null)
    {
        var files = new List<FileInfo>();

        if (!dir.Exists)
            return files;

        var info = new FileInfo(Path.Combine(dir.FullName, $".nozip"));

        if (info.Exists)
        {
            // skip this directory
            writeLine?.Invoke($"{info.DirectoryName}\t({info.Name})");
            return files;
        }

        var options = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var allFiles =
            dir.EnumerateFiles("*.*", options)
            .Where(file => !file.Extension.Equals(".zip", IgnoreCase));

        if(!allFiles.Any(x => x.Exists && x.Length > 0))
        {
            return files;
        }

        info = new FileInfo(Path.Combine(dir.FullName, $"{dir.Name}.zip"));

        using var zip =
            info.Exists ?
            new ZipFile(info.FullName) :
            ZipFile.Create(info.FullName);

        var pass = passwordLookup?.Invoke(dir);
        zip.Password = pass;


        if (zip.IsNewArchive)
        {
            writeLine?.Invoke($"{info.FullName}\t(Creating)");
        }
        else
        {
            writeLine?.Invoke($"{info.FullName}\t(Updating)");
        }


        zip.BeginUpdate();

        try
        {
            foreach (var file in allFiles)
            {
                var entryName = ZipEntry.CleanName(file.GetRelativePath(dir.FullName));
                var entry = zip.GetEntry(entryName);
                if (entry != null)
                {
                    continue; // already in zip
                }

                // add it
                writeLine?.Invoke($"{entryName}\t(Added)");
                zip.Add(file.FullName, entryName);
                files.Add(file);


                // look for a non-matching file in the zip with a different name.
                if (file.Length > 0)
                {
                    //verify by crc too.
                    uint crc = file.GetCrc32();
                    var otherMatches = zip.FindByCrc(crc)
                        .Where(x => x.Name != entryName) // skip the one we just added
                        .Where(x => file.FileMatchesZipEntry(zip, x));
                    foreach (var match in otherMatches)
                    {
                        writeLine?.Invoke($"{entryName}\t({match.Name} matches but named different)");
                        zip.Delete(match.Name);
                    }
                }

            }
        }
        finally
        {
            if (files.Count > 0 || zip.IsNewArchive)
            {
                // Have to wait to rename files back until after zip.Save() is called when the files actually get read.
                zip.CommitUpdate();
                writeLine?.Invoke($"{zip.Name}\t(Saved)");
            }

            // final step to dispose..
            zip.Close();
        }
        return files;
    }

    public static IEnumerable<ZipEntry> FindByCrc(this ZipFile zip, long crc)
    {
        foreach (ZipEntry entry in zip)
        {
            if (entry.Crc == crc)
                yield return entry;
        }
    }

    /// <summary>Remove files in the file system that are already in the zip file.</summary>
    public static IEnumerable<FileSystemInfo> RemoveFilesAlreadyInZip(this FileInfo zipInfo, Func<DirectoryInfo, string?>? passwordLookup = null)
    {
        if (!zipInfo.Exists)
            yield break;

        DirectoryInfo rootDir = zipInfo.Directory!;

        using var zip = new ZipFile(zipInfo.FullName);
        zip.Password = passwordLookup?.Invoke(rootDir);

        var allDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (ZipEntry zFile in zip)
        {
            var dirPath = Path.GetDirectoryName(zFile.Name);

            if (zFile.IsDirectory)
            {
                allDirs.Add(Path.Combine(rootDir.FullName, zFile.Name));
                continue;
            }
            else if(!string.IsNullOrEmpty(dirPath))
            {
                // grab the subfolder.
                allDirs.Add(Path.Combine(rootDir.FullName, dirPath));
            }

            // Check for the file in the directory
            var file = new FileInfo(Path.Combine(rootDir.FullName, zFile.Name));
            if (!file.Exists || zFile.Size == 0)
            {
                continue;
            }
         
            if (file.Length != zFile.Size)
            {
                // file is not the same as the zip entry
                Trace.WriteLine($"File Size Different: {zFile.Name} ({zFile.Size} vs {file.Length} bytes)");
                continue;
            }

            using var fStream = file.OpenRead();
            var fCrc = fStream.GetCrc32();

            if (fCrc != zFile.Crc)
            {
                // file is not the same as the zip entry
                Trace.WriteLine($"CRC different: {zFile.Name} ({zFile.Crc} vs {fCrc} bytes)");
                continue;
            }

            fStream.Position = 0;
            if (fStream.StreamsAreEqual(zip.GetInputStream(zFile)))
            {
                // file is the same as the zip entry
                Trace.WriteLine($"Delete Match: {file.FullName} ({file.Length} bytes {fCrc} crc)");
                fStream.Close();
                file.Delete();
                yield return file;
                continue;
            }
        }//foreach
        
        if(allDirs.Count > 0)
        {
            foreach (var path in allDirs.OrderDescending())
            {
                DirectoryInfo? childDir = new(path);
                foreach(var item in rootDir.DeleteEmptySubDirectories(childDir))
                {
                    Trace.WriteLine($"Deleted Directory: {item.FullName}");
                    yield return item;
                }
            }
        }
    }

    /// <summary>Delete empty subdirectories of the parent directory starting with the child directory</summary>
    /// <param name="parent">Root Directory (will not be deleted if empty)</param>
    /// <param name="child">Subdirectory of Root to Delete up the stack until not empty or hits the parents (root) directory</param>
    /// <returns>Directories deleted</returns>
    public static IEnumerable<DirectoryInfo> DeleteEmptySubDirectories(this DirectoryInfo parent, DirectoryInfo child)
    {
        DirectoryInfo? currentDir = child;
        
        while (currentDir != null && currentDir.IsSubdirectoryOf(parent))
        {
            if (currentDir.Exists && !currentDir.HasFilesOrDirectories())
            {
                currentDir.Delete();
                yield return currentDir;
            }
            currentDir = currentDir.Parent;
        }
    }

    public static bool IsSubdirectoryOf(this DirectoryInfo dir, DirectoryInfo parent)
    {
        if (dir == null || parent == null)
            return false;

        return
            dir.FullName.Length > parent.FullName.Length &&
            dir.FullName.StartsWith(parent.FullName, IgnoreCase);
    }
  
    public static bool HasFilesOrDirectories(this DirectoryInfo dir)
        => dir.Exists && dir.EnumerateFileSystemInfos().Any();

    public static string GetRelativePath(this FileInfo file, string root)
    {
        var path =
            file.FullName.Length > root.Length && file.FullName[root.Length] == Path.DirectorySeparatorChar && file.FullName.StartsWith(root, IgnoreCase) ?
            file.FullName[(root.Length + 1)..] :
            file.FullName;
        return path;
    }

    public static uint GetCrc32(this byte[] bytes)
    {
        var hasher = new System.IO.Hashing.Crc32();
        hasher.Append(bytes);
        return hasher.GetCurrentHashAsUInt32();
    }

    public static uint GetCrc32(this FileInfo file)
    {
        using var stream = file.OpenRead();
        return stream.GetCrc32();
    }

    public static uint GetCrc32(this Stream stream)
    {
        var hasher = new System.IO.Hashing.Crc32();
        hasher.Append(stream);
        return hasher.GetCurrentHashAsUInt32();
    }


    /// <summary>Compare a file to an entry in a Zip File</summary>
    /// <param name="file">File to compare</param>
    /// <param name="zip">Zip file with possible match</param>
    /// <param name="zipEntryName">relative file path in zip file</param>
    /// <returns></returns>
    public static bool FileMatchesZipEntry(this FileInfo file, ZipFile zip, string zipEntryName)
    {
        bool areEqual = false;
        var zipEntry = zip.GetEntry(zipEntryName);
        if (zipEntry == null)
            return false;

        if (file.Exists && file.Length == zipEntry.Size)
        {
            Stream? zipStream = null, fileStream = null;
            try
            {
                zipStream = zip.GetInputStream(zipEntry);
                fileStream = file.OpenRead();

                areEqual = zipStream.StreamsAreEqual(fileStream);
            }
            finally
            {
                zipStream?.Close();
                fileStream?.Close();
            }
        }

        return areEqual;
    }

    /// <summary>Compare a file to an entry in a Zip File</summary>
    /// <param name="file">File to compare</param>
    /// <param name="zip">Zip file with possible match</param>
    /// <param name="zipEntryName">relative file path in zip file</param>
    /// <returns></returns>
    public static bool FileMatchesZipEntry(this FileInfo file, ZipFile zip, ZipEntry zipEntry)
    {
        bool areEqual = false;

        if (file.Exists && zipEntry != null && file.Length == zipEntry.Size)
        {
            Stream? zipStream = null, fileStream = null;
            try
            {
                zipStream = zip.GetInputStream(zipEntry);
                fileStream = file.OpenRead();

                areEqual = zipStream.StreamsAreEqual(fileStream);
            }
            finally
            {
                zipStream?.Close();
                fileStream?.Close();
            }
        }

        return areEqual;
    }

    public static bool StreamsAreEqual(this Stream fs1, Stream fs2)
    {
        if (fs1 == fs2)
            return true;

        const int BYTES_TO_READ = 64 * 1024;
        byte[] one = new byte[BYTES_TO_READ];
        byte[] two = new byte[BYTES_TO_READ];

        int totalBytes1 = 0, totalBytes2 = 0;
        int bytesRead;

        while (ReadBoth())
        {
            for (int i = 0; i < bytesRead; i++)
            {
                if (one[i] != two[i])
                    return false;
            }
        }

        return totalBytes1 == totalBytes2;

        bool ReadBoth()
        {
            bytesRead = fs1.Read(one, 0, one.Length);
            totalBytes1 += bytesRead;

            int bytesRead2 = fs2.Read(two, 0, two.Length);
            totalBytes2 += bytesRead2;

            return bytesRead > 0 && bytesRead == bytesRead2;
        }
    }

}
