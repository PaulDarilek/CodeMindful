using FileSignatures;

namespace ContentManagement;

public static class FileTypeExtensions
{

    /// <summary>Get the format of the file based on the bytes in the file</summary>
    public static FileFormat? GetFileFormat(this FileInfo file)
    {
        var inspector = new FileFormatInspector();
        var format = inspector.DetermineFileFormat(file.OpenRead());
        return format;
    }

    public static string? GetFileFormatExtension(this FileInfo file)
        => file.GetFileFormat()?.Extension;



}
