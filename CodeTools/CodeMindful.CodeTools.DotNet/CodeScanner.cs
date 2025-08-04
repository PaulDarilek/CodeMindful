using System.Collections.Generic;
using System.IO;

namespace CodeMindful.CodeTools.DotNet;

public class CodeScanner
{
    public static CodeScanner Instance { get; private set; }
    static CodeScanner() { Instance = new CodeScanner(); }
    private CodeScanner() { }

    public static IEnumerable<SolutionFile> FindSolutionFiles(string path, bool scanSubfolders)
    {
        SearchOption option = scanSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var files = Directory.EnumerateFiles(path, "*.sln", option);
        foreach (string filePath in files)
        {
            yield return new SolutionFile(filePath);
        }
    }

}
