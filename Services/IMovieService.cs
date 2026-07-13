using myApp.DTOs.Movies;
using myApp.Helpers.Pagination;
using myApp.Helpers.QueryParameters;

namespace myApp.Services;

public interface IMovieService
{
    Task<PagedResult<MovieReadDto>> GetAllAsync(MovieQueryParameters parameters);
    Task<MovieDetailDto?> GetByIdAsync(int id);
    Task<MovieReadDto> CreateAsync(MovieCreateDto dto);
    Task<bool> UpdateAsync(int id, MovieCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
