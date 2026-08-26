using Ganss.Xss;

namespace MyApp.Services;

public interface ISanitizerService
{
    string SanitizeString(string? input);
}

public class SanitizerService : ISanitizerService
{
    private readonly IHtmlSanitizer _sanitizer;
    private readonly ILogger<SanitizerService> _logger;

    public SanitizerService(ILogger<SanitizerService> logger)
    {
        _logger = logger;
        _sanitizer = new HtmlSanitizer{KeepChildNodes = true};

        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedCssProperties.Clear();
    }
    
    public string SanitizeString(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        var sinitize = _sanitizer.Sanitize(input);
        
        if(input != sinitize)
            _logger.LogWarning("Sanitize string: {input} -> {sinitize}", input, sinitize);
        
        return sinitize;
    }
}