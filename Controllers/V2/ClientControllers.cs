namespace myApp.Controllers.V2;

using Asp.Versioning;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Clients;
using MyApp.Models;

[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/clients")]
public class ClientController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ClientController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
        var clientDtos = await _context.Clients
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
}