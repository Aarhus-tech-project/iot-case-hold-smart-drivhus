using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Infrastructure.Common.Services;

// Todo Move settings
public class TwilioSettings
{
    public string TwilioAccountId { get; set; } = string.Empty;
    public string TwilioAuthToken { get; set; } = string.Empty;
    public string FromSenderId { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } // Unsure of what to set. 
}

public class SmsService(IOptions<TwilioSettings> options, IStatusService statusService) : ISmsService
{
    private readonly TwilioSettings settings = options.Value;

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        if (settings.IsEnabled)
        {
            TwilioClient.Init(settings.TwilioAccountId, settings.TwilioAuthToken);

            var messageResult = MessageResource.Create(
                new PhoneNumber(phoneNumber),
                from: settings.FromSenderId,
                body: $"{message}"
            );

            statusService.Write($"Sending sms: {message}");
        }
        else
        {
            statusService.Write($"Sms disabled. Would send: {message}");
        }
    }
}