namespace ApiSDH.Common.Interfaces.Services;

public interface INotificationService
{
    Task ReceiveAnalysis(Guid entityId, string data);
}