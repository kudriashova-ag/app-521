namespace myApp.DTOs.Movies;

public class MovieReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int Year { get; set; }
    public string? PosterFileName { get; set; }
}
