using MetadataExtractor.Util;
using System.Diagnostics;
using ToolBox.Files;

namespace MediaTools.Media;


public static class MediaHelper
{
    /// <summary>Recurse Files/Directories</summary>
    public static bool Recurse { get; set; }

    /// <summary>Force a Reparsing MetaData and Recalc of CRC for a file</summary>
    public static bool Recalc { get; set; }

    /// <summary>Dash is primary separator</summary>
    private const char DashSeparator = MediaMetaData.DashSeparator;
    private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

    public static HashSet<string> IgnoreFileNames { get; } 
    public static HashSet<string> RenameRemoveParts { get; }

    private static HashSet<char> OrientationChars { get; }

    /// <summary>Static Constructor</summary>
    [DebuggerStepThrough]
    static MediaHelper()
    {
        OrientationChars = [.. new char[] {
            MediaMetaData.OrientationEqual,
            MediaMetaData.Landscape,
            MediaMetaData.Portrait,
            char.ToUpper(MediaMetaData.OrientationEqual),
            char.ToUpper(MediaMetaData.Landscape),
            char.ToUpper(MediaMetaData.Portrait),
        }];

        IgnoreFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        RenameRemoveParts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    }

    /// <summary>Rename Duplicate Files in a directory</summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static int RenameMediaFiles(this DirectoryInfo dir, TextWriter? stdOut)
    {
        if (!dir.Exists)
        {
            return 0;
        }

        int renameCount = 0;
        var options = Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        var fileList =
            dir.EnumerateFiles("*", options)
            .Where(file => file.IsMediaFile() && (Recalc || !file.NameIsImageFormat()))
            .ToList();

        foreach (var file in fileList)
        {
            var metaData = file.RenameMediaFile(stdOut);
            if (metaData != null && !file.Name.Equals(metaData.FileInfo.Name, IgnoreCase))
            {
                renameCount++;
            }
        }
        return renameCount;
    }

    public static MediaMetaData? RenameMediaFile(this FileInfo file, TextWriter? stdOut)
    {
        if (!file.IsMediaFile() || (Recalc == false && file.NameIsImageFormat()))
        {
            return null;
        }

        var metaData = new MediaMetaData(file).ReadFromFile();

        // Look for an Existing File...
        Debug.Assert(metaData.Width != null && metaData.Height != null && metaData.FileType != FileType.Unknown);
        var fileNamePrefix = metaData.GetFileNamePrefix();
        if (fileNamePrefix == null)
        {
            return metaData; //audio files or not-processed type files
        }

        // First look for an existing matching file...
        var match = metaData.FileInfo.FindMatchedFiles(searchPattern: metaData.GetFileSearchPattern()).FirstOrDefault();
        if (match != null)
        {
            stdOut?.WriteLine($"Delete Duplicate File: {metaData.FileInfo.FullName} (deleted) --> {match.Name} (matches)");
            metaData.FileInfo.Delete();

            // set the reference to the new file name.
            metaData = new MediaMetaData(metaData, match);
            return metaData;
        }

        // not found using the prefix...
        metaData = SetFileName(metaData, fileNamePrefix, stdOut);
        return metaData;
    }

    /// <summary>Removes Duplicate Media files by using Media File Prefixes</summary>
    /// <returns>Count of Files Deleted</returns>
    public static int RemoveDuplicatesByName(this DirectoryInfo dir, TextWriter stdOut, TextReader stdIn)
    {
        if (!dir.Exists)
        {
            return 0;
        }

        var dupePrefixes =
            dir.EnumerateFiles()
            .Where(NameIsImageFormat)
            .Select(x => x.Name[..^x.Extension.Length][^8..]) // remove the extension and get the last 8 chars
            .GroupBy(x => x)
            .Where(grp => grp.Count() > 1)
            .Select(grp => grp.Key)
            .ToList();

        if(dupePrefixes.Count == 0)
        {
            stdOut.WriteLine($"{dir.FullName} (No Duplicates Detected)");
            return 0;
        }

        stdOut.WriteLine($"{dir.FullName} ({dupePrefixes.Count} Duplicates Detected)");

        int deleteMatchCount = 0;

        foreach (var key in dupePrefixes)
        {
            var searchPattern = $"*{key}.*";
            var matches = dir.EnumerateFiles(searchPattern).ToList();
            stdOut.WriteLine($"\nPossible Duplicates for {searchPattern}");
            foreach (var item in matches)
            {
                stdOut.WriteLine($"\t{item.Name}  ({item.Length} bytes)");
            }

            stdOut.WriteLine($"\nEnter file to Keep from the Duplicates: ");
            var fileNameToKeep = stdIn.ReadLine();
            var keeper =
                !string.IsNullOrWhiteSpace(fileNameToKeep) ?
                matches.FirstOrDefault(x => x.Name.Equals(fileNameToKeep, IgnoreCase)) :
                null;

            if (keeper != null)
            {
                var keepFileBytes = File.ReadAllBytes(keeper.FullName);

                foreach (var item in matches)
                {
                    if (item.Name == keeper.Name)
                    {
                        stdOut.WriteLine($"\tKeep: {item.Name}");
                    }
                    else
                    {
                        if (keeper.IsEqualBytes(item, keepFileBytes))
                        {
                            stdOut.WriteLine($"\tDelete: {item.Name} (duplicate)");
                            item.Delete();
                            deleteMatchCount++;
                        }
                        else
                        {
                            stdOut.WriteLine($"\tSkip: {item.Name} (bytes are different)");
                        }

                    }
                }
            }

        } // dupe key

        stdOut.WriteLine($"\n{dir.FullName} ({deleteMatchCount} matches deleted)\n");
        return deleteMatchCount;
    }

    public static void MergeMediaFiles(this DirectoryInfo dirTo, DirectoryInfo dirFrom, TextWriter stdOut)
    {
        if (!dirTo.Exists)
        {
            stdOut.WriteLine($"Directory Not Found: {dirTo.FullName}");
            return;
        }
        if (!dirFrom.Exists)
        {
            stdOut.WriteLine($"Directory Not Found: {dirFrom.FullName}");
            return;
        }
        if (dirTo.FullName.Equals(dirFrom.FullName, IgnoreCase))
        {
            stdOut.WriteLine($"Directories are the same:\n\t{dirTo.FullName}\n\t{dirFrom.FullName}");
            return;
        }

        int renameCount = 0;

        var recurseBefore = Recurse;
        try
        {
            Recurse = false;
            renameCount += RenameMediaFiles(dirTo, stdOut);
            renameCount += RenameMediaFiles(dirFrom, stdOut);
        }
        finally
        {
            Recurse = recurseBefore;
        }

        int deleteMatchCount = 0;
        int copyNewFileCount = 0;

        foreach (var file in dirFrom.EnumerateFiles())
        {
            if (IgnoreFileNames.Contains(file.Name) || !file.NameIsImageFormat())
                continue;

            string searchPattern = $"{file.Name[..12]}*{file.Name[^8..]}{file.Extension}";

            var match = 
                file.FindMatchedFiles(dirTo).FirstOrDefault() ??
                file.FindMatchedFiles(dirTo, searchPattern).FirstOrDefault();
            
            if (match != null)
            {
                stdOut.WriteLine($"Deleted {file.FullName} from match at {match.FullName}");
                file.Delete();
                deleteMatchCount++;
                continue;
            }

            if (file.Exists)
            {
                var copyTo = new FileInfo(Path.Combine(dirTo.FullName, file.Name));
                if (!copyTo.Exists)
                {
                    stdOut.WriteLine($"Move file {file.FullName} to {copyTo.FullName}");
                    file.MoveTo(copyTo.FullName);
                    copyNewFileCount++;
                }
            }
        }
        stdOut.WriteLine($"File from {dirFrom.FullName} to {dirTo.FullName} {renameCount} files renamed, {deleteMatchCount} matches deleted, {copyNewFileCount} new files");
    }

    public static bool IsMediaFile(this FileInfo file) => MediaMetaData.MediaFileExtensions.ContainsKey(file.Extension) && !IgnoreFileNames.Contains(file.Name);

    [DebuggerStepThrough]
    public static bool NameIsImageFormat(this FileInfo fileInfo)
    {
        // 01234567890123456789
        // L-1920-1080-other-0099AAFF.jpg
        // L-1920-1080-0099AAFF.jpg
        string name = Path.GetFileNameWithoutExtension(fileInfo.Name);

        bool isProcessed =
            name.Length >= 20 &&
            OrientationChars.Contains(name[0]) &&       //  Equal, Landscape, or Portrait
            name[1] == DashSeparator &&
            name.Substring(2, 4).All(char.IsDigit) &&   // Width 4 digits
            name[6] == DashSeparator &&
            name.Substring(7, 4).All(char.IsDigit) &&   // Height 4 digits
            name[11] == DashSeparator &&                //name[11] could be also be name[^9] if there is no extra parts.
            name[^9] == DashSeparator &&  
            name[^8..].All(char.IsAsciiHexDigit);       // Last 8 chars are crc hex digits
        return isProcessed;
    }

    private static MediaMetaData SetFileName(MediaMetaData metaData, string fileNamePrefix, TextWriter? stdOut)
    {
        if (!metaData.FileInfo.Exists)
        {
            return metaData;
        }

        var nameWithoutExt = Path.GetFileNameWithoutExtension(metaData.FileInfo.Name);
        if(nameWithoutExt.StartsWith(fileNamePrefix) && nameWithoutExt.EndsWith(metaData.Crc32Formatted))
        {
            return metaData;
        }

        string searchPattern = metaData.GetFileSearchPattern();
        var match = metaData.FileInfo.FindMatchedFiles(searchPattern: searchPattern).FirstOrDefault();
        if (match != null)
        {
            stdOut?.WriteLine($"Delete Duplicate File: {metaData.FileInfo.FullName}  --> {match.Name}");
            metaData.FileInfo.Delete();

            // set the reference to the new file name.
            metaData = new MediaMetaData(metaData, match);
            return metaData;
        }

        var newFile = CalcFileName(metaData, fileNamePrefix);
        if (metaData.FileInfo.Name.Equals(newFile.Name, IgnoreCase))
        {
            return metaData;
        }

        if (!newFile.Exists)
        {
            stdOut?.WriteLine($"Rename file : {metaData.FileInfo.DirectoryName}  {metaData.FileInfo.Name} --> {newFile.Name}");
            //Debug.Assert(metaData.FileInfo.Name.Length == newFile.Name.Length, $"File Name Lengths are different: {metaData.FileInfo.Name} {newFile.Name}");
            metaData.FileInfo.MoveTo(newFile.FullName); 
            return metaData;
        }

        // eliminate duplicate.
        if (metaData.FileInfo.IsEqualBytes(newFile, null))
        {
            // delete the file being renamed.
            stdOut?.WriteLine($"Delete Duplicate File: {metaData.FileInfo.FullName}  --> {newFile.Name}");
            metaData.FileInfo.Delete();

            // set the reference to the new file name.
            metaData = new MediaMetaData(metaData, newFile);
            return metaData;
        }

        // Bytes are different
        stdOut?.WriteLine($"Duplicate File with Different bytes: {metaData.FileInfo.FullName}  --> {newFile.Name}");
        string fileNameNoExt = Path.GetFileNameWithoutExtension(newFile.Name);
        for (int i = 1; i < 99; i++)
        {
            var newName = $"{fileNameNoExt[..^9]}({i})-fileNameNoExt[^8..]{newFile.Extension}";
            newFile = new FileInfo(Path.Combine(newFile.Directory!.FullName, newName));
            if (!newFile.Exists)
            {
                metaData.FileInfo.MoveTo(newFile.FullName);
                break;
            }
        }
        return metaData;
    }

    //[DebuggerStepThrough]
    private static FileInfo CalcFileName(MediaMetaData metaData, string fileNamePrefix)
    {
        string userNamePart = GetAlphabeticName(metaData, fileNamePrefix);
        Debug.WriteLineIf(!string.IsNullOrEmpty(userNamePart), $"{metaData.FileInfo.FullName} {userNamePart}");

        var extension = metaData.FileType.GetCommonExtension() ?? metaData.FileInfo.Extension;
        extension = extension.Length > 0 && extension[0] != '.' ? '.' + extension : extension;

        var newFile = new FileInfo(Path.Combine(metaData.FileInfo.DirectoryName!, $"{fileNamePrefix}{userNamePart}{metaData.Crc32Formatted}{extension}"));
        return newFile;
    }

    //[DebuggerStepThrough]
    private static string GetAlphabeticName(MediaMetaData metaData, string fileNamePrefix)
    {
        // name without extension
        Span<char> name = metaData.FileInfo.Name[..^metaData.FileInfo.Extension.Length].ToCharArray().AsSpan();
     
        // remove all non alpha numeric characters.
        for (int i = 0; i < name.Length; i++)
        {
            bool isValidChar = char.IsAsciiLetterOrDigit(name[i]) || name[i] == DashSeparator || (i == 0 && name[i] == MediaMetaData.Underscore);
            if (!isValidChar)
            {
                name[i] = DashSeparator;
            }
        }

        // old convention used _ or x between digits and E for equal r
        if (name.Length >= 12 && name[1] == DashSeparator && name[11] == DashSeparator)
        {
            if (char.ToUpper(name[0]) == 'E')
            {
                name[0] = MediaMetaData.OrientationEqual;
                Debug.Assert(false);
            }

            if (name[6] == MediaMetaData.Underscore || name[6] == 'x')
            {
                name[6] = DashSeparator;
                Debug.Assert(false);
            }
        }

        // now we can remove the orientation/width/height prefix.
        if (fileNamePrefix.Length > 0 && name.ToString().StartsWith(fileNamePrefix, IgnoreCase))
        {
            name = name[fileNamePrefix.Length..];
        }

        string crc32fmt = metaData.Crc32Formatted;

        var parts =
            name.ToString()
            .Split(DashSeparator)
            .Where(part => !string.IsNullOrWhiteSpace(part) && !part.Equals(crc32fmt, IgnoreCase))
            .ToList();

        for (int i = parts.Count - 1; i >= 0; i--)
        {
            string currentPart = parts[i];

            if (RenameRemoveParts.Contains(currentPart))
            {
                parts.RemoveAt(i);
                continue;
            }


            if (currentPart.All(char.IsDigit))
            {
                parts.RemoveAt(i);
                continue;
            }

            if (currentPart.All(char.IsAsciiHexDigit) && currentPart.Length > 2 && currentPart.Length % 2 == 0)
            {
                parts.RemoveAt(i);
                continue;
            }

            // 0000x0000x0000  Width x Height x Length mmss
            if (currentPart.Length == 14 && char.ToLower(currentPart[4]) == 'x' && char.ToLower(currentPart[9]) == 'x')
            {
                var chars = currentPart.ToCharArray();
                chars[4] = '0';
                chars[9] = '0';
                if (chars.All(char.IsDigit))
                {
                    parts.RemoveAt(i);
                    continue;
                }

            }

        }

        return parts.Count == 0 ? string.Empty : $"{string.Join($"{DashSeparator}", parts)}{DashSeparator}";
    }


}
