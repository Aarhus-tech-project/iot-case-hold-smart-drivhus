using System.Text.Json;
using Application.Common.Interfaces.Services;

namespace ApiSDH.MIddleware;

/// <summary>
///     ExceptionHandlingMiddleware will handle errors that happen in httpContext/api request flows. When someone calls an
///     endpoint. This middleware is used.
///     This middleware does not handle errors in hosted services or app startup.
/// </summary>
public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    ISmsService smsService,
    IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            // Send sms to admin on error if enabled.
            var notifyAdmin = config.GetValue<bool>("SmsLogger:Enabled");
            if (notifyAdmin)
            {
                var number = config.GetValue<string>("SmsLogger:Number");
                await smsService.SendSmsAsync(number, "App error");
            }

            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            message = "An unexpected error occurred.",
            details = exception.Message // For production, return user-friendly message 
        });

        return context.Response.WriteAsync(result);
    }
}