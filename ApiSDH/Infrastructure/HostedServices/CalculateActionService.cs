using Application.Common.Events;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.HostedServices;

public class CalculateActionService(IServiceScopeFactory scopeFactory, IMediator mediator, ISmsService smsService)
    : BackgroundService
{
    private DateTimeOffset? latestDataSetLowSmsDate;
    private DateTimeOffset? latestWaterTankLowSmsDate;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();

            // TODO calculate action


            var user = await db.Users.FirstOrDefaultAsync(cancellationToken);

            if (user is null) // throws app error, user inforcer burde have kørt 
                throw new InvalidOperationException();

            // Dobble check at det er sorteret korrekt 

            var dataSet = await db.SensorReadings.OrderBy(sr => sr.CreatedAt).Take(20).ToListAsync(cancellationToken);

            if (dataSet.Count is 0)
            {
                if (!latestDataSetLowSmsDate.HasValue ||
                    DateTimeOffset.UtcNow - latestDataSetLowSmsDate > TimeSpan.FromMinutes(15))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber,
                        "Dataset is low on data, api can not calculate action yet.");
                    latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
                }

                goto end;
            }

            if (CheckWater(user, dataSet))
                if (!latestWaterTankLowSmsDate.HasValue ||
                    DateTimeOffset.UtcNow - latestWaterTankLowSmsDate > TimeSpan.FromMinutes(15))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber, "Water level is low, please refill water tank.");
                    latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;
                }

            await mediator.Publish(new PreformActionEvent("parse data here"), cancellationToken);
            end: ;

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }


    public bool CheckWater(UserInfo user, List<SensorReading> dataSet)
    {
        if (user.WaterLimit >= dataSet.Average(sr => sr.WaterLimit)) return true;
        return false;
    }
}