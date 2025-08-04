using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Icc;
using MetadataExtractor.Formats.Jfif;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.QuickTime;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.Util;
using System.Diagnostics;

namespace MediaTools.Media;


public sealed class MediaMetaData
{
    public FileInfo FileInfo { get; }
    public uint? Crc32 { get; set; }
    public string Crc32Formatted => Crc32 != null ? $"{Crc32:x8}" : string.Empty;

    public FileType FileType { get; private set; }
    private IReadOnlyList<MetadataExtractor.Directory>? DirectoryList { get; set; }
    public Dictionary<string, string> Tags { get; }

    public DateTime? CreatedDate { get; private set; }
    public DateTime? ModifiedDate { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public string? Comment { get; private set; }
    public string? Copyright { get; private set; }
    public string? Make { get; private set; }
    public string? Model { get; private set; }
    public int? ThumbnailWidth { get; private set; }
    public int? ThumbnailHeight { get; private set; }

    /// <summary>Unknown Orientation</summary>
    public const char Underscore = '_';         
    public const char Landscape = 'l';
    public const char OrientationEqual = 'o';  //Orientation is not Landcape (x > y) or Portrait (x < y)
    public const char Portrait = 'p';
    public const char DashSeparator = '-';

    /// <summary>Default Constructor</summary>
    [DebuggerStepThrough]
    public MediaMetaData(FileInfo fileInfo)
    {
        FileInfo = fileInfo;
        FileType = FileType.Unknown;
        DirectoryList = null;
        Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Copy to another file Constructor</summary>
    [DebuggerStepThrough]
    public MediaMetaData(MediaMetaData other, FileInfo? fileInfo = null)
    {
        FileInfo = fileInfo ?? other.FileInfo;
        FileType = other.FileType;
        DirectoryList = other.DirectoryList;
        Tags = other.Tags;
        CreatedDate = other.CreatedDate;
        ModifiedDate = other.ModifiedDate;
        Width = other.Width;
        Height = other.Height;
        Duration = other.Duration;
        Comment = other.Comment;
        Copyright = other.Copyright;
        Make = other.Make;
        Model = other.Model;
        ThumbnailWidth = other.ThumbnailWidth;
        ThumbnailHeight = other.ThumbnailHeight;
    }

    public MediaMetaData ReadFromFile()
    {
        if(!FileInfo.Exists || FileInfo.Length == 0) return this;
        if (FileType != FileType.Unknown && DirectoryList != null && Crc32 != null) return this;

        using (FileStream stream = FileInfo.OpenRead())
        {
            if (FileType == FileType.Unknown)
            {
                stream.Position = 0;
                FileType = FileTypeDetector.DetectFileType(stream);
            }

            if (DirectoryList == null)
            {
                stream.Position = 0;
                DirectoryList = ImageMetadataReader.ReadMetadata(stream);
            }   

            if (Crc32 == null)
            {
                stream.Position = 0;
                Crc32 = stream.ToHashCrc32();
            }

            stream.Close();
        }
        return ParseDirectoryList();
    }

    public bool IsMediaFile() => FileType != FileType.Unknown || MediaFileExtensions.ContainsKey(FileInfo.Extension);
    public static bool IsMediaFile(FileInfo file) => MediaFileExtensions.ContainsKey(file.Extension);

    public bool IsVideo() => FileType.GetMimeType()?.StartsWith("video") == true;
    
    public bool IsAudio() => FileType.GetMimeType()?.StartsWith("audio") == true;
    
    public bool IsImageMimeType() => FileType.GetMimeType()?.StartsWith("image") == true;

    public static bool IsMediaFileExtension(FileInfo file) => MediaFileExtensions.ContainsKey(file.Extension);
    
    public static Dictionary<string, FileType> MediaFileExtensions { get; } = GetMediaFileExtensions();
    
    public static Dictionary<string, FileType> GetMediaFileExtensions()
    {
        var dict = new Dictionary<string, FileType>(StringComparer.OrdinalIgnoreCase);
        foreach (FileType filetype in Enum.GetValues(typeof(FileType)))
        {
            // get extensions with period added.
            string[] extensions = filetype.GetAllExtensions()?.ToArray() ?? [];

            foreach (var ext in extensions)
            {
                if(!string.IsNullOrEmpty(ext))
                    dict.TryAdd(ext[0] == '.' ? ext : '.' + ext, filetype);
            }
        }
        return dict;
    }

    public string? GetFileNamePrefix()
    {
        // ensure we have the data from the file.
        if (FileType == FileType.Unknown || Width == null || Height == null) 
           ReadFromFile();

        return GetFileNamePrefix(FileType, Width, Height);
    }

    public static string? GetFileNamePrefix(FileType fileType, int? width, int? height, char unknownOrientation = Underscore)
    {
        char orientation =
            width == null || height == null ? unknownOrientation :
            width > height ? Landscape :
            width < height ? Portrait :
            OrientationEqual;

        switch (fileType)
        {
            case FileType.Unknown:
                // ignore...
                break;

            // image/*
            case FileType.Jpeg:
            case FileType.Png:
            case FileType.WebP:
                return $"{orientation}{DashSeparator}{(width ?? 0):d4}{DashSeparator}{(height ?? 0):d4}{DashSeparator}";


            // Image
            case FileType.Tiff:
            case FileType.Psd:
            case FileType.Bmp:
            case FileType.Gif:
            case FileType.Ico:
            case FileType.Pcx:
            case FileType.Riff:
            case FileType.Arw:
            case FileType.Crw:
            case FileType.Cr2:
            case FileType.Nef:
            case FileType.Orf:
            case FileType.Raf:
            case FileType.Rw2:
            case FileType.Netpbm:
            case FileType.Crx:
            case FileType.Eps:
            case FileType.Tga:
            case FileType.Heif:
                break;

            // Audio Formats
            case FileType.Wav:
            case FileType.Mp3:
                break;

            // Video Formats
            case FileType.Avi:
            case FileType.QuickTime:
            case FileType.Mp4:
                return $"{orientation}{DashSeparator}{(width ?? 0):d4}{DashSeparator}{(height ?? 0):d4}{DashSeparator}";

            default:
                break;
        }

        return null;
    }


    public string GetFileSearchPattern()
    {
        string prefix = GetFileNamePrefix() ?? string.Empty;

        // get extension with leading period.
        string extension = FileType.GetCommonExtension() ?? FileInfo.Extension;
        extension = extension.Length > 0 && extension[0] != '.' ? '.' + extension : extension;

        return $"{prefix}*{Crc32Formatted}{extension}";
    }

    public static string GetFileSearchPattern(FileInfo file, uint? crc32 = null)
    {
        FileType fileType = MediaFileExtensions.TryGetValue(file.Extension, out var type) ? type : FileType.Unknown;
        
        string prefix = GetFileNamePrefix(fileType, null, null, '?')?.Replace('0', '?') ?? string.Empty;

        string crc = crc32 != null ? $"{crc32:x8}" : string.Empty;

        string extension = fileType.GetCommonExtension() ?? file.Extension;
        extension = extension.Length > 0 && extension[0] != '.' ? '.' + extension : extension;

        return $"{prefix}*{crc}{extension}";
    }

    //[DebuggerStepThrough]
    private MediaMetaData ParseDirectoryList()
    {
        if (DirectoryList == null)
        {
            return this;
        }

        foreach (MetadataExtractor.Directory dir in DirectoryList)
        {

            if (dir is JpegDirectory jpgDir)
            {
                AddTags(dir);
                Width = jpgDir.GetImageWidth();
                Height = jpgDir.GetImageHeight();
                Debug.Assert(Width > 0 && Height > 0); // Reset values?
            }

            else if (dir is JfifDirectory)
            {
                AddTags(dir);
                ThumbnailWidth = dir.TryGetByte(JfifDirectory.TagThumbWidth, out var width) ? width : ThumbnailWidth;
                ThumbnailHeight = dir.TryGetByte(JfifDirectory.TagThumbHeight, out var height) ? height : ThumbnailHeight;
            }

            else if (dir is JpegCommentDirectory)
            {
                Comment = dir.GetDescription(JpegCommentDirectory.TagComment);
            }

            else if (dir is ExifDirectoryBase exifBase)
            {
                AddTags(dir);
                Parse(exifBase);
            }

            else if (dir is PngDirectory)
            {
                AddTags(dir);
                DebugDir(dir);

                Width =
                    Width != null ? Width :
                    (int)Math.Ceiling(dir.GetDouble(PngDirectory.TagImageWidth));

                Height =
                    Height != null ? Height :
                    (int)Math.Ceiling(dir.GetDouble(PngDirectory.TagImageHeight));

                Debug.Assert(Width > 0 && Height > 0); // Reset values?
            }

            else if (dir is QuickTimeMovieHeaderDirectory)
            {
                AddTags(dir);
                CreatedDate = dir.GetDateTime(QuickTimeMovieHeaderDirectory.TagCreated);
                ModifiedDate = dir.GetDateTime(QuickTimeMovieHeaderDirectory.TagModified);
                Duration = (TimeSpan?)dir.GetObject(QuickTimeMovieHeaderDirectory.TagDuration);
            }

            else if (dir is QuickTimeTrackHeaderDirectory)
            {
                AddTags(dir);
                CreatedDate = dir.GetDateTime(QuickTimeTrackHeaderDirectory.TagCreated);
                ModifiedDate = dir.GetDateTime(QuickTimeTrackHeaderDirectory.TagModified);

                int width = (int)Math.Ceiling((decimal?)dir.GetObject(QuickTimeTrackHeaderDirectory.TagWidth) ?? decimal.Zero);
                int height = (int)Math.Ceiling((decimal?)dir.GetObject(QuickTimeTrackHeaderDirectory.TagHeight) ?? decimal.Zero);
                //Debug.Assert(width == 0 && height == 0); // Reset values?

                TimeSpan duration = TimeSpan.FromMilliseconds(dir.GetUInt32(QuickTimeTrackHeaderDirectory.TagDuration));

                Width = Width > 0 ? Width : width;
                Height = Height > 0 ? Height : height;

                Duration =
                    Duration == null ? duration :
                    duration > Duration ? duration :
                    Duration;

            }

            else if(dir is AppleMakernoteDirectory)
            {
                AddTags(dir);
                DebugDir(dir);
            }


            // Ignore types...
            else if (dir is FileTypeDirectory ||
                dir is HuffmanTablesDirectory ||
                dir is IccDirectory ||
                dir is QuickTimeFileTypeDirectory ||
                dir is QuickTimeMetadataHeaderDirectory ||
                dir is XmpDirectory)
            {
                // ignore.
            }

            else if (dir is WebPDirectory webpDir)
            {
                AddTags(dir);
                DebugDir(dir);

                Width =
                    Width != null ? Width :
                    (int)Math.Ceiling(webpDir.GetDouble(WebPDirectory.TagImageWidth));

                Height =
                    Height != null ? Height :
                    (int)Math.Ceiling(webpDir.GetDouble(WebPDirectory.TagImageHeight));

                Debug.Assert(Width > 0 && Height > 0); // Reset values?
            }


            else
            {
                AddTags(dir);
                Trace.WriteLine($"Unhandled Type: {dir.GetType().Name}");
                DebugDir(dir);
                // unhandled....
                Debug.Assert(false);
            }
        }

        return this;


    }

    private void Parse(ExifDirectoryBase dir)
    {
        if (dir is ExifIfd0Directory)
        {
            CreatedDate = dir.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var parsedDate) ? parsedDate : CreatedDate;

            Width =
                Width != null ? Width :
                (int)Math.Ceiling(dir.GetDouble(ExifDirectoryBase.TagImageWidth));

            Height =
                Height != null ? Height :
                (int)Math.Ceiling(dir.GetDouble(ExifDirectoryBase.TagImageHeight));

            Make = dir.GetString(ExifDirectoryBase.TagMake);
            Model = dir.GetString(ExifDirectoryBase.TagModel);

            Debug.Assert(Width > 0 && Height > 0); // Reset values?

            Comment = dir.GetDescription(ExifDirectoryBase.TagUserComment);
            Copyright = dir.GetDescription(ExifDirectoryBase.TagCopyright);
        }

        else if(dir is ExifSubIfdDirectory)
        {
            DebugDir(dir);
        }

        else if (dir is ExifThumbnailDirectory)
        {
            DebugDir(dir);
        }

        else if (dir is GpsDirectory ||
            dir is ExifInteropDirectory)
        {
            // ignore
        }

        else // some other Exif sub-class
        {
            Trace.WriteLine($"Unhandled Type: {dir.GetType().Name}");
            DebugDir(dir);
            // unhandled....
            Debug.Assert(false);
        }
    }

    private void AddTags(MetadataExtractor.Directory dir)
    {
        foreach (var tag in dir.Tags)
        {
            if (!string.IsNullOrWhiteSpace(tag.Description))
            {
                string key = $"{dir.Name}.{tag.Name}";
                Tags[key] = tag.Description;
            }
        }
    }

    private void DebugDir(MetadataExtractor.Directory dir)
    {
        Debug.WriteLine($"{FileInfo.Name} metadata {dir.GetType().Name}");

        foreach (Tag tag in dir.Tags)
        {
            object? tagRaw = dir.GetObject(tag.Type);
            Debug.WriteLineIf(tagRaw == null, $"Type {tag.Name} is NULL");
            if (tagRaw != null)
            {
                object tagValue =
                    tagRaw is string[] strArr ?
                    string.Join(", ", strArr) :
                    tagRaw;

                Debug.WriteLine($"({tag.Type}) {tag.Name} = {tagValue} ({tagRaw.GetType().Name})");
            }
        }
    }


}
