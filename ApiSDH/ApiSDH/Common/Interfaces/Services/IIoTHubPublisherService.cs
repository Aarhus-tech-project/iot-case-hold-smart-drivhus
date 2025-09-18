namespace ApiSDH.Common.Interfaces.Services;

/// <summary>
///     Presentation layer interfaces have their implementations in the same layer.
/// </summary>
/// <remarks>
///     <see cref="IIoTHubPublisherService" /> is used to publish events to the arduino via an azure iot hub.
/// </remarks>
public interface IIoTHubPublisherService
{
    Task PublishAsync(string data, CancellationToken cancellationToken = default);
}