using System.ComponentModel.DataAnnotations;

namespace MyApp.Models;

public class Client
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Phone]
    public string Phone { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public string? PhotoFileName { get; set; }   // '435b5-dg4fgd-sdf4.jpg'
    
}