using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myApp.Configuration;
using myApp.Services.Files;

[ApiController]
[Route("api/demo")]
public class DemoController(
    IFileStorageService fileStorage,
    IOptions<JwtOptions> options,
    IOptionsSnapshot<JwtOptions> optionsSnapshot,
    IOptionsMonitor<JwtOptions> optionsMonitor
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
            Options =  options.Value.AccessTokenMinutes,
            Snapshot = optionsSnapshot.Value.AccessTokenMinutes,
            Monitor = optionsMonitor.CurrentValue.AccessTokenMinutes,
            ProcessStarted = Process.GetCurrentProcess().StartTime
        });
    } 



}
