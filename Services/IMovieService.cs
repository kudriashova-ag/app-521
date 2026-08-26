using myApp.DTOs.Movies;
using myApp.Helpers.Pagination;
using myApp.Helpers.QueryParameters;
using myApp.Services.Files;

namespace myApp.Services;

public interface IMovieService
{
    Task<PagedResult<MovieReadDto>> GetAllAsync(MovieQueryParameters parameters);
    Task<MovieDetailDto?> GetByIdAsync(int id);
    Task<MovieReadDto> CreateAsync(MovieCreateDto dto);
    Task<bool> UpdateAsync(int id, MovieCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<MovieReadDto?> UploadPosterAsync(int id, IFormFile poster);
    Task<int?> AddAttachmentAsync(int movieId, IFormFile file, CancellationToken ct = default);
    Task<FileDownload?> GetAttachmentAsync(int attachmentId, CancellationToken ct = default);

}
