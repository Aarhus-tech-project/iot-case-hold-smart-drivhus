using ApiSDH.Common.Interfaces.Services;
using Application.Common.Events;
using MediatR;

namespace ApiSDH.Common.EventHandlers;

public class PreformActionEventHandler(IMqttPublisherService mqttPublisher)
    : INotificationHandler<PreformActionEvent>
{
    public async Task Handle(PreformActionEvent notification, CancellationToken cancellationToken)
    {
        //await hub.Clients.All.ReceiveAnalysis(notification.EntityId, notification.Data);

        // MQTT publish
        await mqttPublisher.PublishAsync(notification.EntityId, notification.Data, cancellationToken);
    }
}