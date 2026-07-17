namespace myApp.Services.Files;

public interface IFileStorageService
{
    Task<string> SaveAsync(IFormFile file, string folder, FileVisibility visibility);
    void Delete(string folder, string fileName, FileVisibility visibility);
    Stream? OpenRead(string folder, string fileName, FileVisibility visibility);
}
