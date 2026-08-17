using MyApp.Models;

namespace myApp.Services.Auth;

public interface ITokenService
{
    Task<AccessTokenResult> CreateAccessTokenAsync(ApplicationUser user, CancellationToken ct = default);
}

public record AccessTokenResult(string Token, DateTime ExpiresAtUtc);