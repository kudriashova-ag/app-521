namespace myApp.Services.Files;

public sealed record FileDownload(Stream Download, string ContentType, string DownloadName);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(IFormFile file, string folder, FileVisibility visibility);
    void Delete(string folder, string fileName, FileVisibility visibility);
    Task<FileDownload?> OpenRead(string folder, string fileName, FileVisibility visibility);
}
