using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Movies;
using myApp.Exceptions;
using myApp.Helpers.Pagination;
using myApp.Helpers.Queryable;
using myApp.Helpers.QueryParameters;
using myApp.Models;
using myApp.Services.Files;
using MyApp.Models;

namespace myApp.Services;

public class MovieService(
    AppDbContext context,
    IMapper mapper,
    IFileStorageService fileStorage,
    IFileUrlBuilder _urls,
    ILogger<MovieService> logger) : IMovieService
{

    private const string PosterFolder = "posters";
    private const string AttachmentFolder = "attachments";

    public async Task<PagedResult<MovieReadDto>> GetAllAsync(MovieQueryParameters qp)
    {
        var query = context.Movies
            .AsNoTracking()   // IQueryable
            .ApplyFilters(qp) // return IQueryable
            .ApplySort(qp.Sort);

        var dto = await query
                .ToPagedResultAsync<Movie, MovieReadDto>(
                qp.Page,
                qp.Size,
                mapper.ConfigurationProvider);

        foreach (var m in dto.Items)
        {
            m.PosterFileName = _urls.PublicUrl(m.PosterFileName, PosterFolder);
        }

        return dto;
    }

    public async Task<MovieDetailDto?> GetByIdAsync(int id)
    {
        logger.LogError("Get movie by id {id}", id);

        var movie = await context.Movies
            .Include(m => m.Director)       // eager loading (жадібне завантаження) 
            .Include(m => m.MovieActors)    // eager loading (жадібне завантаження)
            .ThenInclude(ma => ma.Actor)    // eager loading (жадібне завантаження)
            .Include(m => m.Attachments)

            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new NotFoundException("Movie not found");

        if (movie == null) return null;

        var dto = mapper.Map<MovieDetailDto>(movie);
        dto.PosterFileName = _urls.PublicUrl(movie?.PosterFileName, PosterFolder);
        return dto;
    }

    public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto)
    {
        var movie = mapper.Map<Movie>(dto);
        context.Movies.Add(movie);
        await context.SaveChangesAsync();

        return mapper.Map<MovieReadDto>(movie);
    }

    public async Task<bool> UpdateAsync(int id, MovieCreateDto dto)
    {
        var movie = await context.Movies.FindAsync(id);
        if (movie == null) return false;

        mapper.Map(dto, movie);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await context.Movies.FindAsync(id);
        if (movie == null) return false;

        context.Movies.Remove(movie);
        await context.SaveChangesAsync();
        return true;
    }


    public async Task<MovieReadDto?> UploadPosterAsync(int id, IFormFile poster)
    {
        var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null) return null;

        var oldPoster = movie.PosterFileName;
        var stored = await fileStorage.SaveAsync(poster, PosterFolder, FileVisibility.Public);
        movie.PosterFileName = stored.FileName;
        await context.SaveChangesAsync();

        if (oldPoster is not null)
        {
            fileStorage.Delete(PosterFolder, oldPoster, FileVisibility.Public);
        }

        var dto = mapper.Map<MovieReadDto>(movie);
        dto.PosterFileName = _urls.PublicUrl(movie.PosterFileName, PosterFolder);

        return dto;
    }

    public async Task<int?> AddAttachmentAsync(int movieId, IFormFile file, CancellationToken ct = default)
    {
        var movie = await context.Movies.FindAsync([movieId], ct);
        if (movie is null) return null;

        var stored = await fileStorage.SaveAsync(file, AttachmentFolder, FileVisibility.Private);

        var attachment = new MovieAttachment
        {
            MovieId = movieId,
            StoredFileName = stored.FileName,
            OriginalFileName = stored.OriginalFileName,
            Size = stored.Size
        };
        context.MovieAttachments.Add(attachment);
        await context.SaveChangesAsync();

        return attachment.Id;
    }


    public async Task<FileDownload?> GetAttachmentAsync(int attachmentId, CancellationToken ct = default)
    {
        var att = await context.MovieAttachments.FindAsync([attachmentId], ct);
        if (att is null) return null;

        var download = await fileStorage.OpenRead(AttachmentFolder, att.StoredFileName, FileVisibility.Private);
        if (download is null) return null;
        
        return download with
        {
            DownloadName = att.OriginalFileName,
            ContentType = download.ContentType
        };
    }


}
