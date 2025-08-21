using ApiSDH.Common.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace ApiSDH.Common.Services;

public class NotificationService : Hub<INotificationService>
{
}