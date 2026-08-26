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
using myApp.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi;
using myApp.Behaviors;
using myApp.Configuration;
using Serilog;

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

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<RoleService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
}
);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


var jwt = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(options =>
    {
        // AddIdentity() виставив своєю схемою cookie (Identity.Application), і вона
        // перебиває DefaultScheme. Для Web API cookie не потрібні — задаємо явно.
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // не перейменовувати claims з коротких імен у довгі WS-Federation URI
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwt["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,   // дефолт 5 хв — токен «не протухає» вчасно

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                  Encoding.UTF8.GetBytes(jwt["Key"]!)),

            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = "role"       // живе в парі з MapInboundClaims = false
        };
    });



// FluentValidation  реєстрація всіх валідаторів
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Фільтр валідації для всіх DTO
builder.Services.AddScoped(typeof(ValidationFilter<>));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


builder.Services.AddScoped<IFileUrlBuilder, FileUrlBuilder>();
builder.Services.AddSingleton<IFileStorageService, LocalFileStorageService>();

builder.Host.UseSerilog((context, cfg)=>
    cfg.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", builder =>
    {
        builder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://127.0.0.1:5500");
    });
});


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Please enter token",
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc)] = new List<string>()
    });

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
app.UseCors("FrontendPolicy");

// 8. Аутентифікація та Авторизація
app.UseAuthentication(); // хто ти?  заповнює HttpContext.User
app.UseAuthorization(); // що тобі можна? 


// 9. Маппінг контролерів
app.MapControllers();

app.Run();

