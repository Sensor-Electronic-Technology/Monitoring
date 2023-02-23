using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MonitoringWeb.WebApp.Data;

namespace MonitoringWeb.WebApp.Services; 

public class FileHandlerService {
    private ILogger<FileHandlerService> _logger;
    private IGridFSBucket _bucket;
    
    public FileHandlerService(IMongoClient client, IOptions<MonitorWebsiteSettings> settings,
        ILogger<FileHandlerService> logger) {
        var database = client.GetDatabase(settings.Value.DatabaseName);
        this._bucket = new GridFSBucket(database, new GridFSBucketOptions {
            BucketName = "website_images"
        });
    }

    public async Task UploadNewImage(IFormFile file,string filename) {
        var stream=file.OpenReadStream();
        await this._bucket.UploadFromStreamAsync(filename, stream);
    }

    public async Task DownloadLatestImage(string filename,string path) {
        using (var stream = new FileStream(path, FileMode.Append, 
                   FileAccess.Write)) {
            await this._bucket.DownloadToStreamByNameAsync(filename, stream);
            stream.Close();
        }
    }
}