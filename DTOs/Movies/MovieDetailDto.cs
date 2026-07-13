using myApp.DTOs.Actors;
using myApp.DTOs.Directors;

namespace myApp.DTOs.Movies;

public class MovieDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public int Year { get; set; }

    public DirectorReadDto? Director { get; set; }
    public List<MovieCastMemberDto>? Actors { get; set; }
}
