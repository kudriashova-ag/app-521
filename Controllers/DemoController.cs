using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myApp.Configuration;
using myApp.Integrations.OpenWeather;
using myApp.Services.Files;

[ApiController]
[Route("api/demo")]
public class DemoController(
    IFileStorageService fileStorage,
    IOptions<JwtOptions> options,
    IOptionsSnapshot<JwtOptions> optionsSnapshot,
    IOptionsMonitor<JwtOptions> optionsMonitor,
    IWeatherClient weatherClient
    ) : ControllerBase
{


    [HttpPost("demo")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(1024 * 1024 * 10)]
    public async Task<IActionResult> Demo(IFormFile file)
    {
        var error = FileValidators.ValidateImage(file, 10 * 1024 * 1024);
        if (error is not null) return BadRequest(new { error });

        var fileName = await fileStorage.SaveAsync(file, "test", FileVisibility.Public);

        return Ok(new { fileName });
    }

    [HttpGet("options")]
    public IActionResult GetOptions()
    {
        return Ok(new
        {
            Options = options.Value.AccessTokenMinutes,
            Snapshot = optionsSnapshot.Value.AccessTokenMinutes,
            Monitor = optionsMonitor.CurrentValue.AccessTokenMinutes,
            ProcessStarted = Process.GetCurrentProcess().StartTime
        });
    }


    [HttpGet("weather")]
    public async Task<IActionResult> GetWeather()
    {
        // var client = new HttpClient();
        // var json = await client.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=Dnipro&appid=b3ea3946cd08306b75c8e73b04e6a794&units=metric");
        // var result = await json.Content.ReadAsStringAsync();
        // return Ok(result);

        return Ok(await weatherClient.GetCurrentWeatherAsync("Dnipro"));
    }
}
