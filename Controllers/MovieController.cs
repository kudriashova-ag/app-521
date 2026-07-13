using Microsoft.AspNetCore.Mvc;
using myApp.DTOs.Movies;
using myApp.Helpers.QueryParameters;
using myApp.Services;

[ApiController]
[Route("api/movies")]
[Consumes("application/json")]
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    /// <summary>
    /// Get all movies
    /// </summary>
    /// <returns> List of movies </returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<MovieReadDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMovies([FromQuery] MovieQueryParameters parameters)
    {
        var movies = await _movieService.GetAllAsync(parameters);
        return Ok(movies);
    }

    /// <summary>
    /// Get movie by id
    /// </summary>
    /// <param name="id"> id of movie </param>
    /// <returns> movie </returns>
    [HttpGet("{id}")]
    [ProducesResponseType<MovieDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovie(int id)
    {
        var movie = await _movieService.GetByIdAsync(id);
        return movie == null ? NotFound() : Ok(movie);
    }


    /// <summary>
    /// Create movie
    /// </summary>
    /// <param name="dto"> movie create dto </param>
    /// <returns> movie read dto </returns>
    [HttpPost]
    [ProducesResponseType<MovieReadDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieReadDto>> CreateMovie(MovieCreateDto dto)
    {
        var movie = await _movieService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMovie(int id, MovieCreateDto dto)
    {
        var updated = await _movieService.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var deleted = await _movieService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
