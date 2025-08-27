namespace ApiSDH.Common.Interfaces.Services;

public interface IIoTHubPublisherService
{
    Task PublishAsync(string data, CancellationToken cancellationToken = default);
}