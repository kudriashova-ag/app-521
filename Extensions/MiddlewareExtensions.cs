using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using myApp.Middleware;
using Data;

namespace myApp.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseApplicationMiddleware(this WebApplication app)
    {
        // 1. Обробка винятків
        app.UseExceptionHandler();

        // 2. Swagger (конфігурується окремо)

        // 3. Перенаправлення з http на https
        app.UseHttpsRedirection();

        app.UseMiddleware<RequestLoggingMiddleware>();

        // 4. Статичні файли
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
            RequestPath = "/uploads"
        });

        return app;
    }

    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        // 5. Роутинг явна маршрутизація
        app.UseRouting();

        // 6. Логування (виконується автоматично через middleware)

        // 7. CORS
        app.UseCors("FrontendPolicy");

        // 8. Аутентифікація та Авторизація
        app.UseAuthentication(); // хто ти?  заповнює HttpContext.User
        app.UseAuthorization(); // що тобі можна?

        app.UseRateLimiter();

        // 9. Маппінг контролерів
        app.MapControllers();

        return app;
    }

    public static void InitializeDatabase(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();
            SeedData.Initialize(context);
        }
    }
}
