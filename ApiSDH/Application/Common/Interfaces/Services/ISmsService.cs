namespace Application.Common.Interfaces.Services;

/// <summary>
///     Application Interfaces have their implementations in the infrastructure layer.
///     <see cref="ISmsService" /> is a simple service used to send sms'es to the users.
/// </summary>
public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}