using Application.Common.Interfaces.Services;
using MediatR;

namespace Application.Common.Behaviors;

public class CommandLifecycleBehavior<TRequest, TResponse>(IStatusService statusService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // On Start
        statusService.Write($"➡️ Starting {typeof(TRequest).Name}");

        try
        {
            var response = await next();

            // On Success
            statusService.Write($"✅ Completed {typeof(TRequest).Name}");

            return response;
        }
        catch (Exception ex) // How does this work in regards to global exception handler 
        {
            // On Failure
            statusService.Write($"❌ Failed {typeof(TRequest).Name}: {ex.Message}");
            throw; // rethrow so your global exception handler can still handle it
        }
    }
}