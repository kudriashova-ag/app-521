namespace MyApp.DTOs.Identity;

public record LoginResult(bool Success, bool IsLockedOut, AuthResponseDto? Response);

public record AuthResponseDto(string Token, DateTime ExpiresAtUtc);