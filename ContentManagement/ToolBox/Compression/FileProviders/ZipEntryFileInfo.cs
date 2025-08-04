//using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.FileProviders;

namespace ToolBox.Compression.FileProviders;
using System.IO.Compression;


public class ZipEntryFileInfo : BasicFileInfo, IFileInfo
{
    /// <summary>Path within the zip file, including <see cref="BasicFileInfo.Name"/></summary>
    public string? FullPath { get; set; }
    public string? Comment { get; set; }
    public int Attributes { get; set; }
    public long? CompressedLength { get; set; }
    public bool? IsEncrypted { get; set; }

    public string? Password { private get; set; }
    public bool HasPassword => !string.IsNullOrEmpty(Password);

    public ZipEntryFileInfo() : base()
    {

    }

    public ZipEntryFileInfo(IFileInfo zipFile, ZipArchiveEntry zipEntry, string? password = null) : base(string.Empty, zipFile.Exists, zipEntry.IsDirectory(), zipEntry.Length, zipFile.PhysicalPath ?? string.Empty, zipEntry.LastWriteTime)
    {
        Name = string.Empty;
        Exists = zipFile.Exists;
        IsDirectory = zipEntry.IsDirectory();
        Length = zipEntry.Length;
        PhysicalPath = zipFile.PhysicalPath ?? string.Empty;
        LastModified = zipEntry.LastWriteTime;

        Password = password;
        Name = Path.GetFileName(zipEntry.Name);
        FullPath = zipEntry.Name;
        Comment = zipEntry.Comment;
        Crc32 = zipEntry.Crc32;
        Attributes = zipEntry.ExternalAttributes;
        CompressedLength = zipEntry.CompressedLength;
        IsEncrypted = zipEntry.IsEncrypted;
    }

    public override Stream CreateReadStream()
    {
        if (File.Exists(PhysicalPath))
        {
            using var zip = ZipFile.Open(PhysicalPath, ZipArchiveMode.Read, new System.Text.UTF8Encoding(false));
            string name = FullPath ?? Name;
            var entry = zip.GetEntry(name);

            if (entry != null)
                return entry.Open();
        }
        return Stream.Null;
    }
}


