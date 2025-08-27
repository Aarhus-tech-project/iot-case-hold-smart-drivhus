using MediatR;

namespace Application.Common.Events;

public record PreformActionEvent(string Data) : INotification;
// dont know what data to reutnr yet but this is a fine placeholder 