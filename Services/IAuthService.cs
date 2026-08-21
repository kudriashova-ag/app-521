using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;

namespace MyApp.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<LoginResult> LoginAsync(LoginDto dto);
    Task<string> UserData();
}