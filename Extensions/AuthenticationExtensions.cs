using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using myApp.Configuration;
using System.Text;

namespace myApp.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");

        services.Configure<JwtOptions>(jwtSection);

        services.AddAuthentication(options =>
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
                ValidIssuer = jwtSection["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,   // дефолт 5 хв — токен «не протухає» вчасно

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                      Encoding.UTF8.GetBytes(jwtSection["Key"]!)),

                NameClaimType = JwtRegisteredClaimNames.Sub,
                RoleClaimType = "role"       // живе в парі з MapInboundClaims = false
            };
        });

        return services;
    }
}
