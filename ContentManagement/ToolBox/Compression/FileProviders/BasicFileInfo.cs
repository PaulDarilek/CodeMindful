using Microsoft.Extensions.FileProviders;

namespace ToolBox.Compression.FileProviders;

public class BasicFileInfo : IFileInfo
{
    public const int UnknownLength = -1;

    /// <summary></summary>
    public string Name { get; set; }

    /// <summary></summary>
    public bool Exists { get; set; }

    /// <summary></summary>
    public bool IsDirectory { get; set; }

    /// <summary></summary>
    public long Length { get; set; }

    /// <summary></summary>
    public string PhysicalPath { get; set; }

    /// <summary></summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary></summary>
    public uint? Crc32 { get; set; }

    public BasicFileInfo()
    {
        Name ??= string.Empty;
        PhysicalPath ??= string.Empty;
    }

    public BasicFileInfo(FileSystemInfo info)
    {
        Name = info.Name;
        Exists = info.Exists;
        PhysicalPath = info.FullName;
        LastModified = info.LastWriteTime;
        IsDirectory = info is DirectoryInfo;
        Length =
            !Exists ? UnknownLength :
            (info as FileInfo)?.Length ?? UnknownLength;
    }

    public BasicFileInfo(string name, bool exists, bool isDirectory, long length, string physicalPath, DateTimeOffset lastModified)
    {
        Name = name;
        Exists = exists;
        PhysicalPath = physicalPath;
        LastModified = lastModified;
        IsDirectory = isDirectory;
        Length = exists && !isDirectory ? length : UnknownLength;
    }

    public virtual Stream CreateReadStream()
    {
        if (IsDirectory || !Exists)
        {
            return Stream.Null;
        }
        var info = new FileInfo(PhysicalPath);
        return
            info.Exists ?
            info.OpenRead() :
            Stream.Null;

    }
}
