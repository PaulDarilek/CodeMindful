using Xunit;
using System.IO;
using System.Linq;
using CodeHaptic.CodeTools.DotNet;
using System;
using System.Configuration;
using CodeMindful.CodeTools.DotNet;

namespace CodeHaptic.CodeTools.Test
{
    public class SolutionCleanTests
    {
        private static string SolutionDirSetting = ConfigurationManager.AppSettings[nameof(SolutionDir)];
        private string SolutionDir = Path.GetFullPath(Path.Combine(System.Environment.CurrentDirectory, @"..\..\..\.."));

        [Fact]
        public void LoadsSolutionFile()
        {
            SolutionFile solution = new SolutionFile(Path.Combine(SolutionDir, "CodeHaptic.CodeTools.sln"));
            TestSolutionLoadAndClean(solution);
        }

        [Fact]
        public void LoadsProjectFile()
        {
            var test = new ProjectFile(Path.Combine(SolutionDir, @"SolutionCleaner.Cmd\SolutionCleaner.Cmd.csproj"));

            test.Load();
            test.Clean();
        }


        [Fact]
        public void FindSolutions()
        {
            var solutions = CodeScanner.Instance.FindSolutionFiles(SolutionDir, true).ToArray();
            var count = solutions.Count();
            Assert.True(count > 0);
            foreach (var solution in solutions)
            {
                TestSolutionLoadAndClean(solution);
            }
        }

        [Fact]
        public void FindSolutionsRepo()
        {
            var solutions = CodeScanner.Instance.FindSolutionFiles(Path.Combine(SolutionDir,".."), true).ToArray();
            var count = solutions.Count();
            Assert.True(count > 0);
            foreach (var solution in solutions)
            {
                solution.Load();
                solution.DeleteTestResults();
                solution.DeletePackages();
            }
        }
        private void TestSolutionLoadAndClean(SolutionFile solution)
        {
            solution.Load();

            Assert.NotNull(solution.SolutionGuid);
            Assert.Equal(2, solution.Projects.Count);

            solution.DeleteTestResults();
            solution.DeletePackages();
        }
    }
}
