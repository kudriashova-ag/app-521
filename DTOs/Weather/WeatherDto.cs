namespace myApp.DTOs.Weather;

public record WeatherDto(
    string City,
    double Temperature,
    double FeelsLike,
    double WindSpeedMs,
    string Description);