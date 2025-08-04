namespace ToolBox.Files;

public static class FolderExtensions
{
    public static int DeleteEmptyFolders(DirectoryInfo dir)
    {
        int deleted = 0;
        if (!dir.Exists)
            return deleted;

        var subdirs = new List<DirectoryInfo>();

        var allDirs = dir
            .EnumerateDirectories("*", SearchOption.AllDirectories)
            .Union([dir])
            .OrderByDescending(x => x.FullName)
            .ToArray();

        foreach (var subdir in allDirs)
        {
            if (subdir.Exists && !subdir.EnumerateFileSystemInfos().Any())
            {
                try
                {
                    subdir.Delete();
                    deleted++;
                }
                catch (Exception)
                {
                    // ignore
                }

            }
        }
        return deleted;
    }

}
