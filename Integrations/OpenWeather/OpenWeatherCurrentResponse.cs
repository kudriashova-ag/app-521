using System.Text.Json.Serialization;

namespace myApp.Integrations.OpenWeather;

internal sealed record OpenWeatherCurrentResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("main")]
    public OpenWeatherMain Main { get; init; } = null!;

    [JsonPropertyName("wind")]
    public WindBlock Wind { get; init; } = null!;

    [JsonPropertyName("weather")]
    public IReadOnlyList<WeatherBlock> Weather { get; init; } = null!;
}


internal sealed record OpenWeatherMain
{
    [JsonPropertyName("temp")]
    public double Temp { get; init; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; init; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; init; }
}

internal sealed record WindBlock
{
    [JsonPropertyName("speed")]
    public double Speed { get; init; }
}

internal sealed record WeatherBlock
{
    [JsonPropertyName("main")]
    public string Main { get; init; } = null!;
}