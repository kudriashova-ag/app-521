using Asp.Versioning;
using AutoMapper;
using Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using myApp.Services;
using myApp.Services.Files;
using myApp.Services.Auth;
using MyApp.Models;
using MyApp.Services;
using myApp.Behaviors;
using myApp.Configuration;
using myApp.Filters;
using myApp.ExceptionHandling;
using Serilog;
using System.Threading.RateLimiting;
using System.Security.Claims;

namespace myApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<RoleService>();

        services.AddScoped<ISanitizerService, SanitizerService>();
        services.AddScoped<IFileUrlBuilder, FileUrlBuilder>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        return services;
    }

    public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }

    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddScoped(typeof(ValidationFilter<>));

        return services;
    }

    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(Program));
        return services;
    }

    public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                // Захист сервера - 3req/s загалом
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(1),
                        PermitLimit = 3
                    })
                ),

                // 5req для одного користувача на 10s
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var key = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        Window = TimeSpan.FromSeconds(10),
                        PermitLimit = 5,
                        QueueLimit = 0
                    });
                })
            );
        });

        return services;
    }

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
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

        return services;
    }

    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
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

        return services;
    }

    public static IHostBuilder AddSerilog(this IHostBuilder builder)
    {
        builder.UseSerilog((context, cfg) =>
            cfg.ReadFrom.Configuration(context.Configuration)
        );

        return builder;
    }

    public static IServiceCollection OpenWeatherConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OpenWeatherOptions>()
            .Bind(configuration.GetSection(OpenWeatherOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
