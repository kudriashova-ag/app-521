using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Movies;
using myApp.Exceptions;
using myApp.Helpers.Pagination;
using myApp.Helpers.Queryable;
using myApp.Helpers.QueryParameters;
using myApp.Services.Files;
using MyApp.Models;

namespace myApp.Services;

public class MovieService(
    AppDbContext context,
    IMapper mapper,
    IFileStorageService fileStorage,
    IFileUrlBuilder _urls) : IMovieService
{

    private const string PosterFolder = "posters";

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
        var movie = await context.Movies
            .Include(m => m.Director)       // eager loading (жадібне завантаження) 
            .Include(m => m.MovieActors)    // eager loading (жадібне завантаження)
            .ThenInclude(ma => ma.Actor)    // eager loading (жадібне завантаження)
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

        movie.PosterFileName = await fileStorage.SaveAsync(poster, PosterFolder, FileVisibility.Public);
        await context.SaveChangesAsync();

        if (oldPoster is not null)
        {
            fileStorage.Delete(PosterFolder, oldPoster, FileVisibility.Public);
        }

        var dto = mapper.Map<MovieReadDto>(movie);
        dto.PosterFileName = _urls.PublicUrl(movie.PosterFileName, PosterFolder);

        return dto;
    }
}
