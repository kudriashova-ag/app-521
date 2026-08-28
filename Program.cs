using Microsoft.Extensions.Options;
using myApp.Configuration;
using myApp.Extensions;
using myApp.Integrations.OpenWeather;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери
builder.Services.AddControllers();

// Конфігурація послуг (Services)
builder
    .Services.AddDatabaseContext(builder.Configuration)
    .AddIdentityConfiguration()
    .AddApplicationServices()
    .AddAutoMapperConfiguration()
    .AddMediatRConfiguration()
    .AddValidationServices()
    .AddExceptionHandling()
    .AddVersioning()
    .AddSwaggerConfiguration()
    .AddRateLimitingConfiguration()
    .AddCorsConfiguration()
    .AddJwtAuthentication(builder.Configuration);

// builder.Services.AddOptions<OpenWeatherOptions>()
//  .Bind(builder.Configuration.GetSection(OpenWeatherOptions.SectionName))
//  .ValidateDataAnnotations()
//  .ValidateOnStart();


builder.Services.AddHttpClient<IWeatherClient, WeatherClient>((sp, client) =>
{
    var opt = sp.GetRequiredService<IOptions<OpenWeatherOptions>>().Value;
    client.BaseAddress = new Uri(opt.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


// Додаємо Serilog логування
builder.Host.AddSerilog();

var app = builder.Build();

// Middleware та конфігурація pipeline
app
    .UseSwaggerConfiguration()
    .UseApplicationMiddleware()
    .UseApplicationPipeline();

// Ініціалізація БД та seed даних
app.InitializeDatabase();

app.Run();
