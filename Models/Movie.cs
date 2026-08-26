using myApp.Models;

namespace MyApp.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int Year { get; set; }
    public int? DirectorId { get; set; }
    public string? PosterFileName { get; set; }

    public ICollection<MovieAttachment> Attachments { get; set; } = new List<MovieAttachment>();


    // навігаційна властивість
    public Director? Director { get; set; }
    // навігаційна властивість
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}