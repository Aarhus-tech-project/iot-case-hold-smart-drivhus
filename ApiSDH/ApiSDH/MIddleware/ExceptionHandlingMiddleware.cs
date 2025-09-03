using System.Text.Json;
using Application.Common.Interfaces.Services;

namespace ApiSDH.MIddleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env,
    ISmsService smsService,
    IConfiguration config)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // Proceed to the next middleware or endpoint
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            var notifyAdmin = config.GetValue<bool>("SmsLogger:Enabled");
            if (notifyAdmin)
            {
                var number = config.GetValue<string>("SmsLogger:Number");
                // if valid number 
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