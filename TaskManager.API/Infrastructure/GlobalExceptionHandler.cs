using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.API.Infrastructure;

// Реализация IExceptionHandler, появившегося в .NET 8
// Это позволяет элегантно обрабатывать все неперехваченные исключения
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Логируем саму ошибку
        _logger.LogError(exception, "Произошла необработанная ошибка: {Message}", exception.Message);

        // Формируем стандартизированный ответ ProblemDetails (RFC 7807)
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Внутренняя ошибка сервера",
            Detail = "Произошла непредвиденная ошибка при обработке вашего запроса. Пожалуйста, попробуйте позже.",
            Instance = httpContext.Request.Path
        };

        // Если это специфичное доменное исключение (можно создать свои),
        // здесь можно менять Status Code на 400 или 404.

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Возвращаем true, сигнализируя, что мы обработали исключение
        return true;
    }
}
