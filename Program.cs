using myApp.Extensions;

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
