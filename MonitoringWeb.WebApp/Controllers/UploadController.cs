using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MonitoringWeb.WebApp.Services;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

//using MongoDB

namespace MonitoringWeb.WebApp.Controllers {
    public class ChunkMetadata {
        public int Index { get; set; }
        public int TotalCount { get; set; }
        public int FileSize { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? FileGuid { get; set; }
    }
    [Route("api/[controller]")]
    public class UploadController : Controller {
        private readonly IWebHostEnvironment _hostingEnvironment;
        //FileUrlStorageService _fileUrlStorageService;
        private readonly FileHandlerService _fileHandlerService;
        public UploadController(IWebHostEnvironment hostingEnvironment,FileHandlerService fileHandlerService) {
            _hostingEnvironment = hostingEnvironment;
            //_fileUrlStorageService = fileUrlStorageService;
            this._fileHandlerService = fileHandlerService;
        }

        [HttpPost]
        [Route("UploadFile")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> UploadFile(IFormFile ImageUpload, string chunkMetadata) {
            var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "Images");
            // Removes temporary files
            RemoveTempFilesAfterDelay(tempPath, new TimeSpan(0, 0, 20));
            try {
                if (!string.IsNullOrEmpty(chunkMetadata)) {
                    var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);
                    
                    AppendContentToFile(tempFilePath, ImageUpload);
                    if(metaDataObject.Index == (metaDataObject.TotalCount - 1)) {
                        ProcessUploadedFile(tempFilePath,"GasDetectorMap.png");
                        await this._fileHandlerService.UploadNewImage(ImageUpload, "GasDetectorMap.png");
                    }
                }
            }
            catch (Exception ex) {
                return BadRequest();
            }
            return Ok();
        }
        void RemoveTempFilesAfterDelay(string path, TimeSpan delay) {
            var dir = new DirectoryInfo(path);
            if (dir.Exists)
                foreach (var file in dir.GetFiles("*.tmp").Where(f => f.LastWriteTimeUtc.Add(delay) < DateTime.UtcNow))
                    file.Delete();
        }
        void ProcessUploadedFile(string tempFilePath, string fileName) {
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            var imagePath = Path.Combine(path, fileName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
            System.IO.File.Copy(tempFilePath, imagePath);
        }
        void AppendContentToFile(string path, IFormFile content) {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write)) {
                content.CopyTo(stream);
            }
        }
    }
}