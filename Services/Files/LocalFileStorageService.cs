namespace myApp.Services.Files;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _publicRoot;
    private readonly string _privateRoot;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _publicRoot = Path.Combine(env.ContentRootPath, "uploads");
        _privateRoot = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
    }

    private string RootFor(FileVisibility v) => v == FileVisibility.Public ? _publicRoot : _privateRoot;

    public async Task<string> SaveAsync(IFormFile file, string folder, FileVisibility visibility)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant(); //fsf786fsfcds.jpg
        var directory = Path.Combine(RootFor(visibility), folder); //  /uploads/posters
        Directory.CreateDirectory(directory);
        var fullPath = Path.Combine(directory, fileName); // /uploads/posters/fsf786fsfcds.jpg

        // save
        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await file.CopyToAsync(stream);
        return fileName;
    }

    public void Delete(string folder, string fileName, FileVisibility visibility)
    {
        var path = ResolveSafePath(folder, fileName, visibility);
        if (File.Exists(path)) File.Delete(path);
    }

    public Stream? OpenRead(string folder, string fileName, FileVisibility visibility)
    {
        var path = ResolveSafePath(folder, fileName, visibility);
        return File.Exists(path)
            ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null;
    }

    private string ResolveSafePath(string folder, string fileName, FileVisibility visibility)
    {
        var directory = Path.GetFullPath(Path.Combine(RootFor(visibility), folder));
        var fullPath = Path.GetFullPath(Path.Combine(directory, fileName));

        if (!fullPath.StartsWith(directory + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new InvalidOperationException("Спроба виходу за межі каталогу зберігання.");

        return fullPath;
    }
}