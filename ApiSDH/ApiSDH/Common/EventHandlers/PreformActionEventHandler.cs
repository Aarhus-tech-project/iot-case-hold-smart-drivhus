using ApiSDH.Common.Interfaces.Services;
using Application.Common.Events;
using Application.Common.Interfaces.Services;
using MediatR;

namespace ApiSDH.Common.EventHandlers;

public class PreformActionEventHandler(IIoTHubPublisherService ioTHubPublisherService, IStatusService statusService)
    : INotificationHandler<PreformActionEvent>
{
    /// <summary>
    ///     // Arduino listens for events(<see cref="PreformActionEvent" />). This is how the greenhouse is controlled by the
    ///     backend.
    ///     Event examples: Open window, water plant and water tank low on water.
    /// </summary>
    public async Task Handle(PreformActionEvent notification, CancellationToken cancellationToken)
    {
        await ioTHubPublisherService.PublishAsync(notification.Data, cancellationToken);
        statusService.Write($"Published event: {notification.Data}");
    }
}