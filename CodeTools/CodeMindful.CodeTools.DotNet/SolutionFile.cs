using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeHaptic.CodeTools.DotNet
{
    public class SolutionFile
    {
        public string FilePath { get; private set; }
        public string Text { get; private set; }
        public string SolutionGuid { get; set; }
        public bool IsLoaded { get; private set; }
        public string DirectoryPath
        {
            get
            {
                return FilePath == null ? null : Path.GetDirectoryName(this.FilePath);
            }
        }

        public List<ProjectFile> Projects { get; private set; }

        public SolutionFile(string path)
        {
            this.FilePath = path;
            this.Projects = new List<ProjectFile>();
        }

        public void Load()
        {
            if(File.Exists(this.FilePath))
            {
                this.Text = File.ReadAllText(this.FilePath);
                ParseFileContents();
                this.IsLoaded = true;
            }
                                                                                                           }
        public void Clean()
        {
            DeleteTestResults();
            DeletePackages();
        }

        public void DeleteTestResults()
        {
            var folder = Path.Combine(this.DirectoryPath, "TestResults");
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
            var folder = Path.Combine(this.DirectoryPath, "packages");
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
            var lines = this.Text
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
                    this.SolutionGuid = text.Substring(find.Length, text.Length - find.Length - 1);
                    continue;
                }

                find = "Project(";
                if (text.StartsWith(find))
                {
                    ProjectFile project = ProjectFile.ParseFromSolutionLine(text, this.DirectoryPath);
                    if (project != null)
                        this.Projects.Add(project);
                    continue;
                }
                

            }
        }
    }
}
