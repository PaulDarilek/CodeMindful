using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CodeHaptic.CodeTools.DotNet
{
    public class ProjectFile
    {
        public string FilePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string AssemblyName { get; private set; }
        public string Platform { get; private set; }
        public string OutputType { get; private set; }
        public string RootNamespace { get; private set; }
        public string TargetFrameworkVersion { get; private set; }
        public int FileAlignment { get; private set; }
        public bool AutoGenerateBindingRedirects { get; private set; }
        public string Text { get; private set; }
        public string[] OutputPaths { get; private set; }
        public List<ProjectDependency> Dependencies { get; private set; }

        public ProjectFile(string path)
        {
            this.FilePath = path;
            this.Dependencies = new List<ProjectDependency>();
        }

        public ProjectFile Load()
        {
            if (File.Exists(this.FilePath))
            {
                this.Text = File.ReadAllText(this.FilePath);
                ParseFileContents();
            }
            return this;
        }

        public ProjectFile Clean()
        {

            return this;
        }

        private void ParseFileContents()
        {
            if (string.IsNullOrWhiteSpace(this.Text))
                return;

            try
            {
                XDocument xDoc = XDocument.Parse(this.Text);

                XNamespace ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

                //Project Info
                var projInfo = (
                    from item in xDoc.Descendants(ns + "PropertyGroup")
                    where item.Element(ns + "ProjectGuid") != null
                    select new
                    {
                        ProjectGuid = item.Element(ns + "ProjectGuid").Value,
                        Platform = item.Element(ns + "Platform").Value,
                        OutputType = item.Element(ns + "OutputType").Value,
                        RootNamespace = item.Element(ns + "RootNamespace").Value,
                        AssemblyName = item.Element(ns + "AssemblyName").Value,
                        TargetFrameworkVersion = item.Element(ns + "TargetFrameworkVersion").Value,
                        FileAlignment = item.Element(ns + "FileAlignment").Value,
                        AutoGenerateBindingRedirects = item.Element(ns + "AutoGenerateBindingRedirects").Value,
                    }).FirstOrDefault();
                if (projInfo != null)
                {
                    this.ProjectGuid = projInfo.ProjectGuid;
                    this.AssemblyName = projInfo.AssemblyName;
                    this.Platform = projInfo.Platform;
                    this.OutputType = projInfo.OutputType;
                    this.RootNamespace = projInfo.RootNamespace;
                    this.TargetFrameworkVersion = projInfo.TargetFrameworkVersion;
                    this.FileAlignment =  int.Parse(projInfo.FileAlignment);
                    this.AutoGenerateBindingRedirects = bool.Parse(projInfo.AutoGenerateBindingRedirects);
                }

                this.OutputPaths = (
                    from item in xDoc.Descendants(ns + "PropertyGroup")
                    where item.Element(ns + "OutputPath") != null
                    select item.Element(ns + "OutputPath").Value
                    ).ToArray();


                this.Dependencies.Clear();

                //References "By DLL (file)"
                var refererences = from list in xDoc.Descendants(ns + "ItemGroup")
                            from item in list.Elements(ns + "Reference")
                                /* where item.Element(ns + "HintPath") != null */
                            select new ProjectDependency
                            {
                                ReferenceInclude = item.Attribute("Include").Value,
                                RefType = (item.Element(ns + "HintPath") == null) ? "CompiledDLLInGac" : "CompiledDLL",
                                HintPath = (item.Element(ns + "HintPath") == null) ? string.Empty : item.Element(ns + "HintPath").Value
                            };


                if(refererences.Any())
                {
                    this.Dependencies.AddRange(refererences);
                }


                //References "By Project"
                refererences = from list in xDoc.Descendants(ns + "ItemGroup")
                            from item in list.Elements(ns + "ProjectReference")
                            where
                            item.Element(ns + "Project") != null
                            select new ProjectDependency
                            {
                                ReferenceInclude = item.Attribute("Include").Value,
                                RefType = "ProjectReference",
                                ProjectGuid = item.Element(ns + "Project").Value
                            };

                if (refererences.Any())
                {
                    this.Dependencies.AddRange(refererences);
                }
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    Trace.WriteLine(ex.Message);
                    ex = ex.InnerException;
                }
            }

        }

        public static ProjectFile ParseFromSolutionLine(string line, string rootPath)
        {
            //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SolutionCleaner.Code", "SolutionCleaner.Code\SolutionCleaner.Code.csproj", "{D8C53155-E27B-407A-8727-01CD812AD5B5}"
            if (string.IsNullOrEmpty(line))
                return null;
            line = line.Trim();
            const string projStart = "Project(\"{";
            const string projNext1 = "}\") = ";

            if (line.StartsWith(projStart))
            {
                string guid1 = line.Substring(projStart.Length, 36);
                line = line.Substring(projStart.Length + guid1.Length + projNext1.Length);
                string[] parts = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    string name = parts[0].Replace("\"", "").Trim();
                    string path = parts[1].Replace("\"", "").Trim();
                    string guid2 = parts[2].Replace("\"", "").Trim();

                    return new ProjectFile(System.IO.Path.Combine(rootPath, path)) { ProjectGuid = guid2, AssemblyName = name };
                }
            }
            return null;   
        }
    }

    public class ProjectDependency
    {
        public string ReferenceInclude {get; set;}
        public string RefType { get; set; }
        public string HintPath { get; set; }
        public string ProjectGuid { get; set; }
    }
}
