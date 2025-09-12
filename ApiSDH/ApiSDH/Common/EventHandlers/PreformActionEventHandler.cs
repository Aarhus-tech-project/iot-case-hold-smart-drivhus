using ApiSDH.Common.Interfaces.Services;
using Application.Common.Events;
using Application.Common.Interfaces.Services;
using MediatR;

namespace ApiSDH.Common.EventHandlers;

public class PreformActionEventHandler(IIoTHubPublisherService ioTHubPublisherService, IStatusService statusService)
    : INotificationHandler<PreformActionEvent>
{
    public async Task Handle(PreformActionEvent notification, CancellationToken cancellationToken)
    {
        await ioTHubPublisherService.PublishAsync(notification.Data, cancellationToken);
        statusService.Write($"Published event: {notification.Data}");
    }
}