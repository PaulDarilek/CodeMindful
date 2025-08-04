global using ToolBox.Compression;
using MediaTools.Media;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ToolBox.Console;

namespace MediaTools;

public static class Program
{
    private static FolderZipper Zipper { get; } = new FolderZipper
    {
        Dirs = false,
        Recurse = false,
        WriteLine = Console.Out.WriteLine,
    };

    private const string No = nameof(No);

    private static CommandParser Parser { get; }
        = new CommandParser([
            new CommandType(nameof(MediaHelper.Recalc), (args) => MediaHelper.Recalc = true) {Description = "Force Recalulation of CRC and reading MetaData from file even when file is correctly named."},
            new CommandType(No + nameof(MediaHelper.Recalc), (args) => MediaHelper.Recalc = false) {Description = "Skip Recalulating of CRC and reading MetaData from file when file is correctly named."},
            new CommandType(nameof(Recurse), Recurse) {Description = "Find Files in subdirectories"},
            new CommandType(nameof(NoRecurse), NoRecurse) {Description = "Do not look in subdirectories"},
            new CommandType(nameof(Zipper.RemoveMatch), Zipper.RemoveMatch) {MinParms = 1, MaxParms = 100, Description = "Remove file already in zip file"},
            new CommandType(nameof(Zipper.Dirs), (args) => Zipper.Dirs = true) {Description = $"({nameof(ZipFolder)}) Process each subdir as a zip file."},

            new CommandType(nameof(RenameMediaFiles), RenameMediaFiles) {MinParms = 1, MaxParms = 100, Description = "Rename Media files in a Directory from their MetaData"},
            new CommandType(nameof(RemoveDuplicates), RemoveDuplicates) {MinParms = 1, MaxParms = 100, Description = "Rename media files and identify duplicates for a folder"},
            new CommandType(nameof(MergeMediaFiles), MergeMediaFiles) {MinParms = 2, MaxParms = 2, Description = "Combine Image/Video Files from two folders. (Move/Delete into first)"},
            new CommandType(nameof(DeleteCrc32s), DeleteCrc32s) {MinParms = 1, MaxParms = 100, Description = $"Delete files with CRC32 hashes in the filename listed in the AppSettings.{nameof(AppSettings.DeleteCrc32s)}"},

            new CommandType(nameof(ZipFolder), ZipFolder) {MinParms = 1, MaxParms = 100, Description = $"Zip Files in Folder (Use switch -{nameof(FolderZipper.Dirs)} to only do dirs"},
        ]);

    /// <summary>Constructor</summary>
    static Program()
    {
        MediaMetaData.MediaFileExtensions.Add(".v", MetadataExtractor.Util.FileType.Mp4);
        MediaMetaData.MediaFileExtensions.Add(".p", MetadataExtractor.Util.FileType.Png);
        MediaMetaData.MediaFileExtensions.Add(".j", MetadataExtractor.Util.FileType.Jpeg); 

        MediaHelper.IgnoreFileNames.Add(".nomedia");

        MediaHelper.RenameRemoveParts.UnionWith([
            "botImage",
            "Image",
            "shared",
            "video",
            "E",
            "L",
            "P",
            "V",
            "Ss",
        ]);
    }

    public static void Main(string[] args)
    {
        Parser.Run(args);
    }

    private static void Recurse(IEnumerable<string> ignore)
    {
        MediaHelper.Recurse = true;
        Zipper.Recurse = true;
    }
    
    private static void NoRecurse(IEnumerable<string> ignore)
    {
        MediaHelper.Recurse = false;
        Zipper.Recurse = false;
    }


    private static void RenameMediaFiles(IEnumerable<string> folders)
    {
        var directories = folders.GetValidDirectories();
        foreach (var dir in directories)
        {
            dir.RenameMediaFiles(Console.Out);
        }
    }

    private static void ZipFolder(IEnumerable<string> folders)
    {
        Zipper.ZipFolders(folders);
    }

    private static void RemoveDuplicates(IEnumerable<string> folders)
    {
        var directories = folders.GetValidDirectories();
        foreach (var dir in directories)
        {
            dir.RemoveDuplicatesByName(Console.Out, Console.In);
        }
    }

    private static void MergeMediaFiles(IEnumerable<string> folders)
    {
        var directories = folders.Select(x => new DirectoryInfo(x.Trim())).ToList();

        if (directories.Count != 2)
        {
            Console.Error.WriteLine($"{nameof(MergeMediaFiles)} Requires to Folders, not {directories.Count}");
            return;
        }
        var dirTo = directories[0];
        var dirFrom = directories[1];

        // [0] = directory to copy to, [1] = directory to copy from
        Console.Out.WriteLine($"Merging {dirFrom.FullName} into {dirTo.FullName}");
        dirTo.MergeMediaFiles(dirFrom, Console.Out);

        if (MediaHelper.Recurse)
        {
            int startAt = dirFrom.FullName.Length + 1;
            var relativePaths = dirFrom.GetDirectories("*", SearchOption.AllDirectories).Select(x => x.FullName[startAt..]).ToList();
            foreach (string path  in relativePaths)
            {
                var subDirTo = new DirectoryInfo(Path.Combine(dirTo.FullName, path));
                var subDirFrom = new DirectoryInfo(Path.Combine(dirFrom.FullName, path));
                if (subDirTo.Exists && subDirFrom.Exists)
                {
                    Console.Out.WriteLine($"Merging {subDirFrom.FullName} into {subDirTo.FullName}");
                    subDirTo.MergeMediaFiles(subDirFrom, Console.Out);
                }
            }
        }

    }

    private static void DeleteCrc32s(IEnumerable<string> folders)
    {
        AppSettings settings = GetAppSettings();
        var directories = folders.GetValidDirectories();
        foreach (var dir in directories)
        {
            foreach(string Crc32Hex in settings.DeleteCrc32s)
            {
                var potentialMatches = dir.GetFiles($"*{Crc32Hex}*.*").ToList();
                foreach (var file in potentialMatches)
                {
                    uint crc32 = file.ToHashCrc32();
                    string crcFmt = $"{crc32:x8}";

                    if(crcFmt == Crc32Hex)
                    {
                        Console.Out.WriteLine($"Deleting {file.FullName} that has matching Crc32 {Crc32Hex} ({crc32})");
                        file.Delete();
                    }
                }
            }   
        }
    }


    private static List<DirectoryInfo> GetValidDirectories(this IEnumerable<string> paths)
    {
        var list = new List<DirectoryInfo>();
        foreach (var path in paths)
        {
            var dir = new DirectoryInfo(path.Trim());
            if (dir.Exists)
            {
                list.Add(dir);
                if (MediaHelper.Recurse)
                {
                    list.AddRange(dir.GetDirectories("*", SearchOption.AllDirectories));
                }
            }
        }
        return list;
    }

    public static AppSettings GetAppSettings()
    {
        var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

        IConfiguration config = builder.Build();
        var section = config.GetSection(nameof(AppSettings));
        AppSettings settings = config.GetSection(nameof(AppSettings)).Get<AppSettings>() ?? new AppSettings();
        return settings;
    }


}
