using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Common.Behaviors;

/// <summary>
///     <see cref="CommandLifecycleBehavior{TRequest,TResponse}" /> is a MediatR pipeline behavior that wraps the execution
///     of
///     <see cref="IRequest{TResponse}" /> handlers, allowing status updates to be written at the start, on success, and on
///     failure.
/// </summary>
public class CommandLifecycleBehavior<TRequest, TResponse>(IStatusService statusService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // On Start.
        statusService.Write($"➡️ Starting {typeof(TRequest).Name}");

        try
        {
            var response = await next();

            // On Success.
            statusService.Write($"✅ Completed {typeof(TRequest).Name}");

            return response;
        }
        catch (Exception ex)
        {
            // On Failure
            statusService.Write($"❌ Failed {typeof(TRequest).Name}: {ex.Message}");
            throw; // rethrow so global exception handler can still handle it.
        }
    }
}