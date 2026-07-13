namespace myApp.Helpers.QueryParameters;

// DTO

public record MovieQueryParameters
{
  public int Page { get; set; } = 1;
  public int Size { get; set; } = 10;
  public string? Search { get; set; }
  public string? Genre { get; set; }
  public int? MinYear { get; set; }
  public int? MaxYear { get; set; }
  public string? Sort { get; set; }
}


