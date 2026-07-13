namespace myApp.DTOs.Movies;

public class MovieCastMemberDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
