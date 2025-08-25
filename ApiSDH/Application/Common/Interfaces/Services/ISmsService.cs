namespace Application.Common.Interfaces.Services;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
}