using ApiSDH.Common.Interfaces.Services;
using ApiSDH.Common.Services;
using Application.Common.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ApiSDH.Common.EventHandlers;

public class PreformActionEventHandler(IHubContext<NotificationService, INotificationService> hub)
    : INotificationHandler<PreformActionEvent>
{
    public async Task Handle(PreformActionEvent notification, CancellationToken cancellationToken)
    {
        await hub.Clients.All.ReceiveAnalysis(notification.EntityId, notification.Data);
    }
}