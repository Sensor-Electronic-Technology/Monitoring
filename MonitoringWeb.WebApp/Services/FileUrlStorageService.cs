namespace MonitoringWeb.WebApp.Services; 

public class FileUrlStorageService {
    private Dictionary<Guid, string> FileUrlStorage = new Dictionary<Guid, string>();
    public void Add(Guid fileGuid, string fileUrl) {
        if (!FileUrlStorage.ContainsKey(fileGuid))
            FileUrlStorage.Add(fileGuid, fileUrl);
    }
    public string? Get(Guid fileGuid) {
        return FileUrlStorage.GetValueOrDefault(fileGuid);
    }
}