using System.Net.Mime;
using System.Text.Json;
using FluentValidation;
using UserService.Domain.Extensions;


public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning("Resource not found: {Message}", ex.Message);
            await WriteAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (BadRequestException ex)
        {
            logger.LogWarning("Bad request: {Message}", ex.Message);
            await WriteAsync(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (UnauthorizedException ex)
        {
            logger.LogWarning("Unauthorized access: {Message}", ex.Message);
            await WriteAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (ValidationException ex)
        {
            // Коркарди автоматии хатогиҳои FluentValidation
            logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            
            var errors = ex.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage });
            await WriteAsync(context, StatusCodes.Status400BadRequest, errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            await WriteAsync(context, StatusCodes.Status500InternalServerError, "Internal server error.");
        }
    }

    private static Task WriteAsync(HttpContext ctx, int status, object payload)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = MediaTypeNames.Application.Json;
        
        var response = payload is string msg 
            ? JsonSerializer.Serialize(new { status, error = msg })
            : JsonSerializer.Serialize(new { status, errors = payload });

        return ctx.Response.WriteAsync(response);
    }
}