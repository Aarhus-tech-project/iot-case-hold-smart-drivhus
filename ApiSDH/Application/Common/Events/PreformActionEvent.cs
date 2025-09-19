using MediatR;

namespace Application.Common.Events;

public record PreformActionEvent(string Data) : INotification;