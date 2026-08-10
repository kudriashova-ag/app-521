using System.ComponentModel.DataAnnotations;
using myApp.Validators;

namespace myApp.DTOs.Movies;

public class MovieCreateDto
{
    /// <summary>
    /// Movie title
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Movie genre
    /// </summary>
    public string Genre { get; set; } = null!;

    /// <summary>
    /// Movie year
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Movie director id
    /// </summary>
    public int? DirectorId { get; set; }
}
