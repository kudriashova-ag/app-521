namespace myApp.Controllers.V1;

using Asp.Versioning;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Clients;
using myApp.Services.Files;
using MyApp.Models;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/clients")]
public class ClientController : ControllerBase
{
    private const string PhotoFolder = "photos";

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileUrlBuilder _fileUrlBuilder;



    public ClientController(AppDbContext context, IMapper mapper, IFileStorageService fileStorageService, IFileUrlBuilder fileUrlBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _fileUrlBuilder = fileUrlBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clientDtos = await _context.Clients
            .ProjectTo<ClientReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(clientDtos);
    }

    [HttpPost]
    public async Task<ClientReadDto> CreateClient(ClientCreateDto clientCreateDto)
    {
        var client = _mapper.Map<Client>(clientCreateDto);
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        return _mapper.Map<ClientReadDto>(client);
    }

    [HttpPost("{clientId:int}/photo")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024 * 10)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult?> UploadPhoto(int clientId, IFormFile photo)
    {
        var error = FileValidators.ValidateImage(photo, 10 * 1024 * 1024);
        if (error is not null) return BadRequest(new { error });

        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client is null) return null;

        // service
        var oldPhoto = client.PhotoFileName;

        client.PhotoFileName = await _fileStorageService.SaveAsync(photo, PhotoFolder, FileVisibility.Private);
        await _context.SaveChangesAsync();

        if (oldPhoto is not null)
        {
            _fileStorageService.Delete(PhotoFolder, oldPhoto, FileVisibility.Private);
        }

        var dto = _mapper.Map<ClientReadDto>(client);
        // dto.PhotoFileName = _fileUrlBuilder.EndpointUrl(client.PhotoFileName, PhotoFolder);
        return Ok(dto);
    }


    [HttpGet("{clientId:int}/photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhoto(int clientId)
    {
        var client = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clientId);
        if (client?.PhotoFileName is null) return NotFound();
        
        return null;
       // Response.Headers.Add("Content-Type", "image/jpeg");
       // return Ok({ "message": "success" });
    }




}