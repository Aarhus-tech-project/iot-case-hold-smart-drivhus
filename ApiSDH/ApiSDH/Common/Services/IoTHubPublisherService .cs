using System.Text;
using ApiSDH.Common.Interfaces.Services;
using Microsoft.Azure.Devices;

namespace ApiSDH.Common.Services;

public class IoTHubPublisherService : IIoTHubPublisherService
{
    private readonly ServiceClient _serviceClient;
    private readonly string _targetDeviceId;

    public IoTHubPublisherService(IConfiguration config)
    {
        var connectionString = config["Azure:IotConString"];

        _targetDeviceId = config["Azure:DeviceId"]
                          ?? throw new InvalidOperationException("IoTHubDeviceId is missing from configuration");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("IoTHubConnectionString is missing from configuration");

        _serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
    }

    public async Task PublishAsync(string data, CancellationToken cancellationToken = default)
    {
        var payload = $"Entity: Data: {data}";

        var message = new Message(Encoding.UTF8.GetBytes(payload))
        {
            ExpiryTimeUtc = DateTime.UtcNow.AddMinutes(5) // auto-drop after 5 minutes to avoid que length issues.
        };

        await _serviceClient.SendAsync(_targetDeviceId, message);
    }
}