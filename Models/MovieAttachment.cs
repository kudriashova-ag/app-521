using MyApp.Models;

namespace myApp.Models;

public class MovieAttachment
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public string StoredFileName { get; set; } = null!; // sg4564hdf456sj4564fv.jpg
    public string OriginalFileName { get; set; } = null!; // графік1.jpg
    public long Size { get; set; }
}
