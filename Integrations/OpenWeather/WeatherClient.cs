using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using myApp.Configuration;
using myApp.DTOs.Weather;

namespace myApp.Integrations.OpenWeather;

public class WeatherClient(
    HttpClient _http,
    IOptions<OpenWeatherOptions> _options,
    ILogger<WeatherClient> _logger
) : IWeatherClient
{
    public async Task<WeatherDto?> GetCurrentWeatherAsync(string city, CancellationToken ct = default)
    {
        var url = QueryHelpers.AddQueryString("data/2.5/weather", new Dictionary<string, string?>
        {
            ["q"] = city,
            ["units"] = _options.Value.Units,
            ["lang"] = _options.Value.Lang,
            ["appid"] = _options.Value.ApiKey
        });

        _logger.LogInformation("Getting weather for {url}", url);

        using var response = await _http.GetAsync(url, ct);  // Dispose()

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation("City {City} not found", city);
            return null;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogError("API key is not valid");
            throw new InvalidOperationException("API key is not valid");
        }

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<OpenWeatherCurrentResponse>(cancellationToken: ct);
        if (payload is null)
        {
            throw new InvalidOperationException("Response is null");
        }

        _logger.LogInformation("Got weather for {Payload}", payload.ToString());
        
        return new WeatherDto
        (
            City:  payload.Name,
            Temperature:  Math.Round(payload.Main.Temp, 1),
            FeelsLike: Math.Round(payload.Main.FeelsLike, 1),
            WindSpeedMs:  payload.Wind.Speed,
            Description:  payload.Weather[0].Main
        );

    }
}