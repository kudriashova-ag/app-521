using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using myApp.ExceptionHandling;
using myApp.Filters;
using myApp.Services;
using myApp.Services.Files;
using myApp.Middleware;
using MyApp.Models;
using Microsoft.AspNetCore.Identity;
using MyApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

builder.Services.AddHttpContextAccessor(); // для FileUrlBuilder для отримання запиту

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMovieService, MovieService>();

// FluentValidation  реєстрація всіх валідаторів
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Фільтр валідації для всіх DTO
builder.Services.AddScoped(typeof(ValidationFilter<>));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddSingleton<IFileUrlBuilder, FileUrlBuilder>();
builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options=>
{
    options.SubstituteApiVersionInUrl = true;
    options.GroupNameFormat = "'v'VV";
});

builder.Services.AddEndpointsApiExplorer();

// створення xml файлу для кожної версії
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
// 1. Обробка винятків 
app.UseExceptionHandler();

// 2. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // формує xml файл
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }

    }); // читає xml файл та відображає інтерфейс
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    SeedData.Initialize(context);
}

// 3. Перенаправлення з http на https
app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();


// 4. Статичні файли
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

// 5. Роутинг явна маршрутизація
app.UseRouting();

// 6. Логування

// 7. useCors
app.UseCors();

// 8. Аутентифікація та Авторизація
// app.UseAuthentication();
// app.UseAuthorization();


// 9. Маппінг контролерів
app.MapControllers();

app.Run();

