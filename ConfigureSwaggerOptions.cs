using Asp.Versioning.ApiExplorer; // Тип IApiVersionDescriptionProvider — постачальник інформації про версії API
using Microsoft.Extensions.Options; // Тип IConfigureOptions<T> — механізм конфігурування options-об'єктів
using Microsoft.OpenApi; // Тип OpenApiInfo — опис метаданих Swagger-документа (назва, версія тощо)
using Swashbuckle.AspNetCore.SwaggerGen; // Тип SwaggerGenOptions — налаштування генератора Swagger

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>

{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;

    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "myApp",
                Version = description.ApiVersion.ToString()
            });
        }
    }
}