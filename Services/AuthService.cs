using Microsoft.AspNetCore.Identity;
using myApp.Services.Auth;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser()
        {
            Email = dto.Email,
            UserName = dto.Email.Split('@')[0]
        };
        return await _userManager.CreateAsync(user, dto.Password);
    }

    public async Task<LoginResult> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is null) return new LoginResult(false, false, null);

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            dto.Password,
            lockoutOnFailure: true);
            
        Console.WriteLine("!!!!RESULT");

        Console.WriteLine(result);
        if (!result.Succeeded) return new LoginResult(false, result.IsLockedOut, null);

        var token = await _tokenService.CreateAccessTokenAsync(user);
        return new LoginResult(true, false,  new AuthResponseDto(
            Token: token.Token,
            ExpiresAtUtc: token.ExpiresAtUtc
        ));
    }
}