using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;
using Google.Apis.Requests;


namespace CodeMindful.Cloud.Google;

public class GoogleDrive
{
    public string? ApplicationName { get; set; } 
    private DriveService? Service { get; set; }

    /// <summary>Folder ID</summary>
    public List<string> Parents { get; } = [];

    public GoogleDrive(string apiKey)
    {
        // Create the  Drive service.
        Service = new DriveService(new BaseClientService.Initializer()
        {
            ApplicationName = ApplicationName,
            ApiKey = apiKey,
            GZipEnabled = true,
        });
    }

    public GoogleDrive(FileInfo keyFile)
    {
        if (keyFile.Exists)
        {
            // Load the Service account credentials and define the scope of its access.
            var credential = GoogleCredential
                .FromFile(keyFile.FullName)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            // Create the  Drive service.
            Service = new DriveService(new BaseClientService.Initializer()
            {
                ApplicationName = ApplicationName,
                HttpClientInitializer = credential,
            });
        }

    }

    public string UploadFile(string filePath, IDictionary<string, string>? metaData, string? uploadedName = null) 
        => UploadFileAsync(filePath, metaData, uploadedName).Result;    
    public async Task<string> UploadFileAsync(string filePath, IDictionary<string,string>? metaData, string? uploadedName = null)
    {
        // Upload file Metadata
        var fileMetadata = new File()
        {
            Name = uploadedName ?? Path.GetFileName(filePath),
            Parents = Parents,
            AppProperties = metaData,
        };

        string uploadedFileId;
        // Create a new file on Google Drive
        await using (var fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            // Create a new file, with metadata and stream.
            var request = Service!.Files.Create(fileMetadata, fsSource, "text/plain");
            request.Fields = "*";
            var result = await request.UploadAsync(CancellationToken.None);

            if (result.Status == UploadStatus.Failed)
            {
                throw new FileLoadException($"Error uploading file: {result.Exception?.Message}");
            }

            // the file id of the new file we created
            uploadedFileId = request.ResponseBody?.Id ?? string.Empty;
        }

        return uploadedFileId;
    }

    public void UpdateFile(string fileId, string filePath, IDictionary<string, string>? metaData, string? uploadedName = null)
        => UpdateFileAsync(fileId, filePath, metaData, uploadedName).GetAwaiter().GetResult();

    public async Task UpdateFileAsync(string fileId, string filePath, IDictionary<string, string>? metaData, string? uploadedName = null)
    {
        // Let's change the files name.
        // Note: not all fields are writeable watch out, you cant just send uploadedFile back.
        var updateFileBody = new File()
        {
            Name = uploadedName ?? Path.GetFileName(filePath),
            AppProperties = metaData,
        };

        // Then upload the file again with a new name and new data.
        await using var uploadStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        
        // Update the file id, with new metadata and stream.
        var updateRequest = Service!.Files.Update(updateFileBody, fileId, uploadStream, "text/plain");
        var result = await updateRequest.UploadAsync(CancellationToken.None);

        if (result.Status == UploadStatus.Failed)
        {
            throw new FileLoadException($"Error uploading file: {result.Exception?.Message}");
        }
    }

    public FileList GetFileList()
    {
        try
        {
            // Building the initial request.
            var request = Service!.Files.List();

            // Applying optional parameters to the request.
            //request = (FilesResource.ListRequest)SampleHelpers.ApplyOptionalParms(request, optional);

            var pageStreamer = 
                new PageStreamer<File, FilesResource.ListRequest, FileList, string>(
                    (req, token) => request.PageToken = token,
                    response => response.NextPageToken,
                    response => response.Files);

            var allFiles = new FileList
            {
                Files = [],
            };

            foreach (var result in pageStreamer.Fetch(request))
            {
                allFiles.Files.Add(result);
            }

            return allFiles;
        }
        catch (Exception Ex)
        {
            throw new Exception("Request Files.List failed.", Ex);
        }
    }

    public string DeleteFile(string id)
    {
       var request = Service!.Files.Delete(id);
       var result = request.Execute();
       return result;   
    }
}
