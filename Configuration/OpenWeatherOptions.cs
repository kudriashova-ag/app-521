using System.ComponentModel.DataAnnotations;

namespace myApp.Configuration;

public sealed class OpenWeatherOptions
{
    public const string SectionName = "OpenWeather";

    [Required(AllowEmptyStrings = false, ErrorMessage = "API key is required")]
    public string ApiKey { get; set; } = null!;

    [Required, Url]
    public string BaseUrl { get; set; } = "https://api.openweathermap.org/";
    public string Units { get; set; } = "metric";
    public string Lang { get; set; } = "ua";
}