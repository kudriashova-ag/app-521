using Microsoft.AspNetCore.Mvc;
using myApp.Services.Files;

[ApiController]
[Route("api/demo")]
public class DemoController(IFileStorageService fileStorage) : ControllerBase
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
}
