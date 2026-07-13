using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Movies;
using myApp.Helpers.Pagination;
using myApp.Helpers.Queryable;
using myApp.Helpers.QueryParameters;
using MyApp.Models;

namespace myApp.Services;

public class MovieService(AppDbContext context, IMapper mapper) : IMovieService
{
    public async Task<PagedResult<MovieReadDto>> GetAllAsync(MovieQueryParameters qp)
    {
        var query = context.Movies
            .AsNoTracking()   // IQueryable
            .ApplyFilters(qp) // return IQueryable
            .ApplySort(qp.Sort);

        return await query
                .ToPagedResultAsync<Movie, MovieReadDto>(
                qp.Page,
                qp.Size,
                mapper.ConfigurationProvider);
    }

    public async Task<MovieDetailDto?> GetByIdAsync(int id)
    {
        var movie = await context.Movies
            .Include(m => m.Director)       // eager loading (жадібне завантаження) 
            .Include(m => m.MovieActors)    // eager loading (жадібне завантаження)
            .ThenInclude(ma => ma.Actor)    // eager loading (жадібне завантаження)
            .FirstOrDefaultAsync(m => m.Id == id);

        return movie == null ? null : mapper.Map<MovieDetailDto>(movie);
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
}
