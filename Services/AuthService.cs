using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;
using MyApp.Models;

namespace MyApp.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

    public async Task<SignInResult> LoginAsync(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: true);
        return result;
    }
}