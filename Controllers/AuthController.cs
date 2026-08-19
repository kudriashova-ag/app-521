using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myApp.Filters;
using MyApp.DTOs.Identity;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilter<RegisterDto>))]
    [ProducesResponseType<IdentityResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(result);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilter<LoginDto>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = result.IsLockedOut ? "Аккаунт заблоковано" : "Неправильний логін або пароль",
                Status = StatusCodes.Status401Unauthorized
            });

        }
        return Ok(result.Response);
    }

}