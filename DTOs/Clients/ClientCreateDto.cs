using System.ComponentModel.DataAnnotations;

namespace myApp.DTOs.Clients;

public class ClientCreateDto
{
    [Required, MinLength(2), MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required, MinLength(2), MaxLength(50)]
    public string LastName { get; set; } = null!;
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required, Phone]
    public string Phone { get; set; } = null!;
}