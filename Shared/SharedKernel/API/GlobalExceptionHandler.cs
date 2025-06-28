using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace SharedKernel.API;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Handle invalid JSON in the request body
        if (exception is JsonException jsonEx)
        {
            logger.LogWarning(jsonEx, "Malformed JSON in request");
            var pd = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Malformed JSON in request body",
                Status = StatusCodes.Status400BadRequest,
                Detail = jsonEx.Message,
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = pd.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);
            return true;
        }

        if (exception is BadHttpRequestException badReqEx &&
            badReqEx.StatusCode == StatusCodes.Status400BadRequest)
        {
            logger.LogWarning(badReqEx, "Bad HTTP request");
            var pd = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Invalid request body",
                Status = StatusCodes.Status400BadRequest,
                Detail = badReqEx.Message,
                Instance = httpContext.Request.Path
            };
            httpContext.Response.StatusCode = pd.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);
            return true;
        }

        // Fallback for all other exceptions (Internal server error 500)
        logger.LogError(exception, "Unhandled exception occurred");
        var fallback = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Server failure",
            Status = StatusCodes.Status500InternalServerError,
            // Show in developement only
            //Detail = exception.Message
        };
        httpContext.Response.StatusCode = fallback.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(fallback, cancellationToken);
        return true;
    }
}
