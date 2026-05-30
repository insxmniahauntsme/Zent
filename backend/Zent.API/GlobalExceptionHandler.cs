using FluentValidation;
using Zent.Common;
using Zent.Common.Exceptions.Abstractions;

namespace Zent.API;

internal sealed class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            if (ex is ValidationException validationEx)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                
                var errors = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(e => e.ErrorMessage).ToList());
                
                await context.Response.WriteAsJsonAsync(new
                {
                    Code = ErrorCodes.ValidationFailed,
                    Message = "Validation failed.",
                    Errors = errors,
                });

                return;
            }
            
            context.Response.StatusCode = ex switch
            {
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                ForbiddenException => StatusCodes.Status403Forbidden,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
            
            if (ex is ZentException zentEx)
            {
                logger.LogWarning(ex, "Application exception: {zentEx.Message}", zentEx.Message);
                
                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Code = zentEx.ErrorCode,
                    Message = zentEx.Message
                });
            }
            else
            {   
                logger.LogError(ex, "Unhandled exception: {ex.Message}", ex.Message);
                
                await context.Response.WriteAsJsonAsync(new ErrorResponse
                {
                    Code = ErrorCodes.InternalServerError,
                    Message = "An unexpected error occurred."
                });
            }
        }
    }
}
