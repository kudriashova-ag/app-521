namespace myApp.DTOs.Clients;

public class ClientReadDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName { get; set; } = null!;

    public string? PhotoFileName { get; set; }

}