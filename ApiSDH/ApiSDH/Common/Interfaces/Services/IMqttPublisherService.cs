namespace ApiSDH.Common.Interfaces.Services;

public interface IMqttPublisherService
{
    Task PublishAsync(Guid entityId, string data, CancellationToken cancellationToken = default);
}