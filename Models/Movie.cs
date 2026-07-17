namespace MyApp.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int Year { get; set; }
    public int? DirectorId { get; set; }
    public string? PosterFileName { get; set; }


    // навігаційна властивість
    public Director? Director { get; set; }
    // навігаційна властивість
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}