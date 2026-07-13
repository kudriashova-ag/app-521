using System.ComponentModel.DataAnnotations;
using myApp.Validators;

namespace myApp.DTOs.Movies;

public class MovieCreateDto
{
    /// <summary>
    /// Movie title
    /// </summary>
    [Required, MinLength(1), MaxLength(200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Movie genre
    /// </summary>
    [Required, MaxLength(50)]
    public string Genre { get; set; } = null!;

    /// <summary>
    /// Movie year
    /// </summary>
    [Required, YearRange(1888)]
    public int Year { get; set; }

    /// <summary>
    /// Movie director id
    /// </summary>
    public int? DirectorId { get; set; }
}
