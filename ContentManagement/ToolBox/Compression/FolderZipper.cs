namespace ToolBox.Compression;

public class FolderZipper
{
    /// <summary>Zips the contents of the subdirectories only.</summary>
    public bool Dirs { get; set; } = false;
    public bool Recurse { get; set; } = false;
    public Func<DirectoryInfo, string?>? PasswordLookup { get; set; } = null;
    public Action<string>? WriteLine { get; set; } = null; // System.Console.Out.WriteLine;

    public void ZipFolders(IEnumerable<string> folders)
    {
        foreach (var path in folders)
        {
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                continue;
            }

            if (Dirs)
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    var files = subDir.ZipDirectory(PasswordLookup, Recurse, WriteLine);
                    WriteLine?.Invoke($"{subDir.FullName}\t({files.Count} files added)");
                }
            }
            else
            {
                var files = dir.ZipDirectory(PasswordLookup, Recurse, WriteLine);
                WriteLine?.Invoke($"{dir.FullName}\t({files.Count} files added)");
            }

        }


    }

    public void RemoveMatch(IEnumerable<string> folders)
    {
        foreach (var path in folders)
        {
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                continue;
            }

            var options = Recurse || Dirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (var zipFile in dir.EnumerateFiles("*.zip", options))
            {
                var removed = zipFile.RemoveFilesAlreadyInZip(PasswordLookup);
                foreach (var item in removed)
                {
                    string type = item is DirectoryInfo ? "Directory" : "File";
                    WriteLine?.Invoke($"{item.FullName}\t(Removed {type})");
                }
            }
        }

    }
}
 