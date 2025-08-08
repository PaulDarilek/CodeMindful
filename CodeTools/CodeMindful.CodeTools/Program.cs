using CodeMindful.CodeTools.DataDictionary;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace CodeMindful.CodeTools
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scan SQL Database for Schema Objects and Dependencies!");

            //var connectionString = Environment.ExpandEnvironmentVariables("Data Source=%USERPROFILE%\\Sqlite\\DataDictionary.sqLite");
            var connectionString = 
                SqlServerScanner.BuildConnectionString("Server=.;Database=DataDictionary;");

            var dataDict = new DataContext(connectionString);

            var scanner = new SqlServerScanner(dataDict);

            var connectionStrings = GetConnectionStrings(args);

            foreach (var connString in connectionStrings)
            {
                Console.WriteLine($"Process Schema for: {connString}");
                var dbInstance = await scanner.ReadInformationSchema(connString);
                Console.WriteLine($"Schema for {dbInstance.ServerName}.{dbInstance.CatalogName} processed!");
            }

            Console.WriteLine("Reporting to Markdown!");
            var reportDir = new DirectoryInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "DataDictionary"));
            var reporter = new ReportToMarkDown(dataDict, reportDir);
            reporter.GenerateMarkdown();

            Console.WriteLine("Done!");
        }

        private static IEnumerable<string> GetConnectionStrings(IEnumerable<string> args)
        {
            int count = 0;
            foreach (var arg in args)
            {
                count++;
                Console.WriteLine(arg);
                yield return arg;
            }
            if (count > 0) 
                yield break;

            Console.WriteLine("Enter SQL Connection (or blank line to quit)\n\t(Example: Server=myServer;Database=myDb;)");
            string? connString = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(connString))
            {
                yield return connString;
                Console.WriteLine("Enter SQL Connection (or blank line to quit)\n\t(Example: Server=myServer;Database=myDb;)");
                connString = Console.ReadLine();
            }
        }


    }
}
