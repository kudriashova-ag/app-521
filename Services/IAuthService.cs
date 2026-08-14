using Microsoft.AspNetCore.Identity;
using MyApp.DTOs.Identity;

namespace MyApp.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<SignInResult> LoginAsync(LoginDto dto);
}