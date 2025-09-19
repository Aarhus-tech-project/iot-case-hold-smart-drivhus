namespace Application.Common.Interfaces.Services;

/// <summary>
///     Application Interfaces have their implementations in the infrastructure layer.
///     <see cref="IStatusService" /> is a form of status logger. It writes what is happening in real time in the app to a
///     txt file.
///     It writes things like, the status of api requests/commands, green house instructions and what is being sent by the
///     <see cref="ISmsService" />.
/// </summary>
public interface IStatusService
{
    void Write(string message);
}