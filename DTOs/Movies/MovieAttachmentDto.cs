namespace myApp.DTOs.Movies;

public class MovieAttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = null!;   // оригінальне ім'я, напр. "звіт.pdf"
    public long Size { get; set; }
    public string DownloadUrl { get; set; } = null!; // URL на ендпоінт Download
}