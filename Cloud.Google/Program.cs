using Microsoft.Extensions.Configuration;

namespace CodeMindful.Cloud.Google;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Hello, World! {string.Join(", ", args)}");
        var keyFile = new FileInfo(
            Path.Combine(
                Environment.GetEnvironmentVariable("userprofile") ?? string.Empty,
                ".serviceaccounts",
                "photocleaner2025-service001.json"));
        
        string? apiKey = GetApiKey();

        var drive = keyFile.Exists ? new GoogleDrive(keyFile) :new GoogleDrive(apiKey);

        drive.ApplicationName = "PhotoCleaner2025";

        drive.Parents.Add("1CB0vZeYGWp-ivllHZGzTIoAZY9b5f3ET");

        //TestUploadAndUpdate(drive);

        var list = drive.GetFileList();
        int fileCount = 0;
        foreach (var file in list.Files)
        {
            Console.WriteLine($"{file.Name}, {file.Id}, {file.MimeType}");
            if(file.MimeType != "application/vnd.google-apps.folder")
            {
                fileCount++;
                var result = drive.DeleteFile(file.Id);
                Console.WriteLine(result);
            }
        }

        if (fileCount == 0)
        {
            TestUploadAndUpdate(drive);
        }

    }



    public static void TestUploadAndUpdate(GoogleDrive drive)
    {
        string fileToUpload = "C:\\Temp\\Test.txt";
        string originalText = File.ReadAllText(fileToUpload);
        string id = drive.UploadFile(fileToUpload, metaData: null);

        File.WriteAllText(fileToUpload, originalText + Environment.NewLine + id);
        drive.UpdateFile(id, fileToUpload, metaData: null);
    }

    public static string GetApiKey()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        string value = config["ApiKey"] ?? string.Empty;
        return value;
    }


}
