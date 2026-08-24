using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myApp.Filters;
using MyApp.DTOs.Identity;
using MyApp.Features.Auth.Commands.Register;

namespace MyApp.Controllers;

[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class AuthController(IMediator _mediator) : ControllerBase
{

    [HttpPost("register")]
    [ServiceFilter(typeof(ValidationFilter<RegisterDto>))]
    [ProducesResponseType<IdentityResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterCommand(dto), ct);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return Ok(result);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilter<LoginDto>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(dto), ct);
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

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        /*  var id = User.FindFirstValue("sub");
         var email = User.FindFirstValue("email");
         var isAdmin = User.IsInRole("admin");
         return Ok(new { id, email, isAdmin }); */

        /* var id = User.FindFirstValue("sub");
        var user = await _userManager.FindByIdAsync(id);
        var roles = await _userManager.GetRolesAsync(user); */

        var user = await _mediator.Send(new UserDataQuery(), CancellationToken.None);
        return Ok(new { user });
    }



    [HttpPost("register-and-login")]
    [ServiceFilter(typeof(ValidationFilter<RegisterDto>))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAndRegister(RegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginRegisterCommand(dto), ct);

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