using ApiSDH.Common.Interfaces.Services;
using MQTTnet;
using MQTTnet.Protocol;

namespace ApiSDH.Common.Services;

public class MqttPublisherService : IMqttPublisherService
{
    private readonly IMqttClient _mqttClient;

    public MqttPublisherService()
    {
        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder() // opset med iconfig 
            .WithClientId("ApiPublisher")
            .WithTcpServer("your-broker-host", 1883) // or 8883 for TLS
            .WithCredentials("username", "password") // if required
            .WithCleanSession()
            .Build();

        // _mqttClient.ConnectAsync(options, CancellationToken.None)
        //     .GetAwaiter()
        //     .GetResult();
    }

    public async Task PublishAsync(Guid entityId, string data, CancellationToken cancellationToken = default)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("analysis/results")
            .WithPayload($"Entity: {entityId}, Data: {data}")
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await _mqttClient.PublishAsync(message);
    }
}