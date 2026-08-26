using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myApp.DTOs.Movies;
using myApp.Exceptions;
using myApp.Filters;
using myApp.Helpers.QueryParameters;
using myApp.Services;
using myApp.Services.Files;

[ApiController]
[Route("api/movies")]
[Consumes("application/json")]
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly LinkGenerator _linkGenerator;

    public MovieController(IMovieService movieService, LinkGenerator linkGenerator)
    {
        _movieService = movieService;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    /// Get all movies
    /// </summary>
    /// <returns> List of movies </returns>
    [Authorize]
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
        if (movie is null) return NotFound();
        foreach (var attachment in movie.Attachments)
        {
            attachment.DownloadUrl = _linkGenerator.GetUriByAction(
                HttpContext,
                action: "Download",
                controller: "Movie",
                values: new
                {
                    attachmentId = attachment.Id
                }
                ) ?? throw new InvalidOperationException("Failed to generate download url");
        }
        return Ok(movie);
    }


    /// <summary>
    /// Create movie
    /// </summary>
    /// <param name="dto"> movie create dto </param>
    /// <returns> movie read dto </returns>

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter<MovieCreateDto>))]
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

    [HttpPost("{id:int}/poster")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024 * 10)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadPoster(int id, IFormFile poster)
    {
        var error = FileValidators.ValidateImage(poster, 10 * 1024 * 1024);
        if (error is not null) return BadRequest(new { error });

        var movieDto = await _movieService.UploadPosterAsync(id, poster);  // save file to DB 
        return movieDto is not null ? Ok(movieDto) : NotFound();
    }


    [HttpPost("{movieId:int}/attachments")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024 * 10)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAttachment(int movieId, IFormFile file)
    {
        try
        {
            var attachmentId = await _movieService.AddAttachmentAsync(movieId, file);
            if (attachmentId is null) return NotFound();

            return CreatedAtAction(nameof(Download), new { movieId, attachmentId }, new { attachmentId });
        }
        catch (FileValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("attachments/{attachmentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(int attachmentId, CancellationToken ct)
    {
        var download = await _movieService.GetAttachmentAsync(attachmentId, ct);
        if (download is null) return NotFound();

        // attachment → браузер запропонує зберегти файл під оригінальним ім'ям.
        return File(download.Download, download.ContentType, download.DownloadName);
    }


}
