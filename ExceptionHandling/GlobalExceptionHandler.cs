using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using myApp.Exceptions;

namespace myApp.ExceptionHandling;

/// <summary>
/// Єдина точка обробки всіх необроблених винятків API. Перетворює їх на ProblemDetails (RFC 9457)
/// з правильним статус-кодом, а справжні деталі 500-помилок логує на сервері, не показуючи клієнту.
/// </summary>
public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Ресурс не знайдено"),
            BusinessRuleException => (StatusCodes.Status409Conflict, "Порушено бізнес-правило"),
            FluentValidation.ValidationException => (StatusCodes.Status400BadRequest, "Помилка валідації"),
            _ => (StatusCodes.Status500InternalServerError, "Внутрішня помилка сервера")
        };

        // логуємо ПОВНИЙ виняток на сервері (структуроване логування — Модуль 16)
        logger.LogError(exception, "Необроблений виняток");

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                // Detail показуємо НАЗОВНІ: для 500 не віддаємо ex.Message клієнту
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Сталася непередбачена помилка."
                    : exception.Message
            }
        });
    }
}
