using Application.Common.Interfaces.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Infrastructure.InterfaceImplementations.Services;

public class TwilioSettings
{
    public string TwilioAccountId { get; set; } = string.Empty;
    public string TwilioAuthToken { get; set; } = string.Empty;
    public string FromSenderId { get; set; } = string.Empty;
}

public class SmsService(TwilioSettings settings) : ISmsService   // IOptions<TwilioSettings> options ,  _settings = options.Value;
{
    public async Task
        SendSmsAsync(string phoneNumber,
            string message) // read number from db and hold in shared service to use for preformence.
    {
        TwilioClient.Init(settings.TwilioAccountId, settings.TwilioAuthToken);

        var messageResult = MessageResource.Create(
            new PhoneNumber(phoneNumber),
            from: settings.FromSenderId,
            body: $"{message}"
        );
    }
}