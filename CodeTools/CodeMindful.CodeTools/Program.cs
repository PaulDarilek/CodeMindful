using CodeMindful.CodeTools.DataDictionary;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CodeMindful.CodeTools
{
    internal class Program
    {
        [Obsolete]
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scan SQL Database for Schema Objects and Dependencies!");

            //var connectionString = Environment.ExpandEnvironmentVariables("Data Source=%USERPROFILE%\\Sqlite\\DataDictionary.sqLite");
            var connectionString = "Server=.;Database=DataDictionary;";

            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                Pooling = true,
                MaxPoolSize = 10,
            };

            connectionString = builder.ConnectionString;

            var dataDict = new DataContext(connectionString);

            var scanner = new SqlServerScanner(dataDict);

            foreach (var argument in args)
            {
                var conn = GetSqlConnection(argument).GetAwaiter().GetResult();
                Console.WriteLine($"Scan: {conn.DataSource}.{conn.Database}");
                scanner.ReadInformationSchema(conn).GetAwaiter().GetResult();   
            }

            await scanner.RemoveSystemObjects();

            Console.WriteLine("Reporting to Markdown!");
            var reportDir = new DirectoryInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "DataDictionary"));
            var reporter = new ReportToMarkDown(dataDict, reportDir);
            reporter.GenerateMarkdown();

            Console.WriteLine("Done!");
        }

        [Obsolete]
        public static async Task<SqlConnection> GetSqlConnection(string  connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString)
            {
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                Pooling = true,
                MaxPoolSize = 10,
            };
            Debug.WriteLine($"Server={builder.DataSource} Database={builder.InitialCatalog} ");
            Debug.WriteLine($"{nameof(builder.ConnectionString)} is {nameof(builder.ConnectionString)}");

            var conn = new SqlConnection(builder.ConnectionString);
            try
            {
                await conn.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
            return conn;
        }

    }
}
