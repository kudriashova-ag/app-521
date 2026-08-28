using myApp.DTOs.Weather;

namespace myApp.Integrations.OpenWeather;

public interface IWeatherClient
{
    Task<WeatherDto?> GetCurrentWeatherAsync(string city, CancellationToken ct=default);
}