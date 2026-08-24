namespace MyApp.DTOs.Identity;

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public LoginDto(string email, string password) => (Email, Password) = (email, password);
}