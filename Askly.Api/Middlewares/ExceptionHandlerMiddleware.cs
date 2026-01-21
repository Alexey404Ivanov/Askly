using Askly.Api.ErrorResponses;
using Askly.Application.Exceptions;

namespace Askly.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApplicationExceptionBase ex)
        {
            _logger.LogWarning("Application Error: {Message}", ex.Message);
            await ApplicationExceptionHandle(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Unknown Error: {Message}", ex.Message);
            await UnknownExceptionHandle(context);
        }
    }

    public async Task ApplicationExceptionHandle(
        HttpContext context,
        ApplicationExceptionBase ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ex switch
        {
            PollNotFoundException or
                PollOptionsNotFoundException or
                UserNotFoundException or
                VoteNotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            InvalidPasswordException => StatusCodes.Status401Unauthorized,
            UserAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };
        
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(ex.Message);
    }

    public async Task UnknownExceptionHandle(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.WriteAsJsonAsync("Internal server error");
    }
}