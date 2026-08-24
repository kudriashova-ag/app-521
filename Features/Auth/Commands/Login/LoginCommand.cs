using MediatR;
using Microsoft.AspNetCore.Identity;
using myApp.Services.Auth;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Features.Auth.Commands.Register;

public record LoginCommand(LoginDto Dto) : IRequest<LoginResult>;

public class LoginCommandHandler(
    UserManager<ApplicationUser> _userManager,
    SignInManager<ApplicationUser> _signInManager,
    ITokenService _tokenService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null) return new LoginResult(false, false, null);

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded) return new LoginResult(false, result.IsLockedOut, null);

        var token = await _tokenService.CreateAccessTokenAsync(user);
        return new LoginResult(true, false, new AuthResponseDto(
            Token: token.Token,
            ExpiresAtUtc: token.ExpiresAtUtc
        ));
    }
}
