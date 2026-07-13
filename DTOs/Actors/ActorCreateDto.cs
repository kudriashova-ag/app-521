using System.ComponentModel.DataAnnotations;

namespace myApp.DTOs.Actors;

public class ActorCreateDto
{
    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = null!;
}
