using Microsoft.AspNetCore.Mvc;

namespace RushTodo.Api.Helpers;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var response = GetResponse(exception);
            if (response.StatusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Request {Method} {Path} failed.", context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning(exception, "Request {Method} {Path} returned {StatusCode}.",
                    context.Request.Method, context.Request.Path, response.StatusCode);

            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = response.StatusCode,
                Title = response.Title,
                Detail = response.Detail,
                Instance = context.Request.Path,
            });
        }
    }

    private static ExceptionResponse GetResponse(Exception exception) => exception switch
    {
        ConcurrencyException => new(StatusCodes.Status409Conflict, "Conflict", exception.Message),
        UniqueConstraintException => new(StatusCodes.Status409Conflict, "Conflict", exception.Message),
        NotFoundException => new(StatusCodes.Status404NotFound, "Not Found", exception.Message),
        ValidationException => new(StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
        _ => new(StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred."),
    };

    private record ExceptionResponse(int StatusCode, string Title, string Detail);
}
