using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CodeMindful.CodeTools.DotNet;

[method: DebuggerStepThrough]
public class SolutionFile(string path)
{
    public string FilePath { get; private set; } = path;
    public string Text { get; private set; }
    public string SolutionGuid { get; set; }
    public bool IsLoaded { get; private set; }
    public string DirectoryPath
    {
        get
        {
            return FilePath == null ? null : Path.GetDirectoryName(FilePath);
        }
    }

    public List<ProjectFile> Projects { get; private set; } = [];

    public void Load()
    {
        if(File.Exists(FilePath))
        {
            Text = File.ReadAllText(FilePath);
            ParseFileContents();
            IsLoaded = true;
        }
                                                                                                       }
    public void Clean()
    {
        DeleteTestResults();
        DeletePackages();
    }

    public void DeleteTestResults()
    {
        var folder = Path.Combine(DirectoryPath, "TestResults");
        if (Directory.Exists(folder))
        {
            var subFolders = Directory.EnumerateDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                Directory.Delete(subFolder, true);
            }
        }
    }

    public void DeletePackages()
    {
        var folder = Path.Combine(DirectoryPath, "packages");
        if (Directory.Exists(folder))
        {
            var subFolders = Directory.EnumerateDirectories(folder);
            foreach (string subFolder in subFolders)
            {
                Directory.Delete(subFolder, true);
            }
        }
    }


    private void ParseFileContents()
    {
        var lines = Text
            .Split(Environment.NewLine.ToCharArray())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;
            var text = line.Trim();

            var find = "SolutionGuid = {";
            if (text.StartsWith(find))
            {
                SolutionGuid = text.Substring(find.Length, text.Length - find.Length - 1);
                continue;
            }

            find = "Project(";
            if (text.StartsWith(find))
            {
                ProjectFile project = ProjectFile.ParseFromSolutionLine(text, DirectoryPath);
                if (project != null)
                    Projects.Add(project);
                continue;
            }
            

        }
    }
}
