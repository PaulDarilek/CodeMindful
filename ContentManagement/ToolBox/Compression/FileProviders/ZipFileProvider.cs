//using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Metrics;

namespace ToolBox.Compression.FileProviders;

[method: DebuggerStepThrough]
public class ZipFileProvider(FileInfo info) : BasicFileInfo(info), IFileInfo, IDirectoryContents, IFileProvider
{
    //public string? Password { protected get; set; }
    public string? Comment { get; set; }

    public IEnumerator<IFileInfo> GetEnumerator()
        => ReadZipFile().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ReadZipFile().GetEnumerator();

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var dir = new ZipDirectoryContents()
        {
            SubPath = subpath,
            Files = [.. ReadZipFile(subpath)],
        };
        return dir;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        using var zip = ZipFile.Open(PhysicalPath, ZipArchiveMode.Update);
        //zip.Password = Password;

        var entry = zip.GetEntry(subpath);

        if (entry != null)
            return new ZipEntryFileInfo(this, entry);
        else
            return new BasicFileInfo(subpath, false, false, -1, PhysicalPath, DateTimeOffset.MinValue);
    }

    public IChangeToken Watch(string filter)
        => throw new NotImplementedException();

    public IEnumerable<ZipEntryFileInfo> ReadZipFile(string? subPath = null)
    {
        if (File.Exists(PhysicalPath))
        {
            using var zip = ZipFile.OpenRead(PhysicalPath);
            Comment = zip.Comment;
            //zip.Password = Password;

            foreach (var zipEntry in zip.Entries)
            {
                if (subPath == null || zipEntry.Name.StartsWith(subPath))
                {
                    var info = new ZipEntryFileInfo(this, zipEntry, null);
                    yield return info;
                }

            }
        }
    }


}


