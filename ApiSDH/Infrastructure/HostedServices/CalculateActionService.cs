using Application.Common.Interfaces.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.HostedServices;

public class CalculateActionService(IServiceScopeFactory scopeFactory, IMediator mediator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();

            // TODO calculate action

            // publish stuff to iot hub if an action should be taken 

            //await mediator.Publish(new PreformActionEvent(Guid.NewGuid(), "parse data here"), stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}