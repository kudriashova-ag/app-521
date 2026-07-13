
namespace MyApp.Models;

public class Actor
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}