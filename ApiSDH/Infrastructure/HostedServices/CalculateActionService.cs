using Application.Common.Events;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twilio.Rest.Wireless.V1;

namespace Infrastructure.HostedServices;

public class CalculateActionService(IServiceScopeFactory scopeFactory, IMediator mediator, ISmsService smsService)
    : BackgroundService
{
    private DateTimeOffset latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
    private DateTimeOffset latestSoildMoistureCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;
    private DateTimeOffset latestCo2Check = DateTimeOffset.UtcNow;

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

            if (dataSet.Count < 6)
            {
                if (DateTimeOffset.UtcNow - latestDataSetLowSmsDate > TimeSpan.FromMinutes(15))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber,
                        "Dataset is low on data, api can not calculate action yet.");
                    latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
                }

                goto end;
            }
    
            
            // chain water check and water plant 
            if (CheckWater(user, dataSet))
                if (DateTimeOffset.UtcNow - latestWaterTankLowSmsDate > TimeSpan.FromMinutes(15))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber, "Water level is low, please refill water tank.");
                    latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;
                }

            if (CheckSoilMoisture(user, dataSet))
                if (DateTimeOffset.UtcNow - latestSoildMoistureCheck > TimeSpan.FromMinutes(15))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Soil Moisture low."), cancellationToken);
                    await smsService.SendSmsAsync(user.PhoneNumber, "Soil Moisture low, watering plant.");
                    latestSoildMoistureCheck = DateTimeOffset.UtcNow;
                }


            if (CheckCo2(user, dataSet))
            {
                if (DateTimeOffset.UtcNow - latestCo2Check > TimeSpan.FromMinutes(15))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Co2 low."), cancellationToken); // udluft 
                    await smsService.SendSmsAsync(user.PhoneNumber, "Co2 low, opening window.");
                    latestCo2Check = DateTimeOffset.UtcNow;
                }
            }
            
            // check light level 
            
            
            
            //await mediator.Publish(new PreformActionEvent("parse data here"), cancellationToken);
            end: ;

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
    }


    // check if water tank is in need of more water 
    public bool CheckWater(UserInfo user, List<SensorReading> dataSet)
    {
        if (user.WaterLimit >= dataSet.Average(sr => sr.WaterLevel)) return true;
        return false;
    }

    // Check if we need to water plant
    public bool CheckSoilMoisture(UserInfo user, List<SensorReading> dataSet)
    {
        if (user.SoilMoistureLimit >= dataSet.Average(sr => sr.SoilHumidity)) return true;
        return false;
    }

    public bool CheckCo2(UserInfo user, List<SensorReading> dataSet)
    {
        if(user.Co2Limit >= dataSet.Average(sr=>sr.Co2)) return true;
        return false;
    }

    public bool CheckLightLevel(UserInfo user, List<SensorReading> dataSet)
    {
        return true; // send sms med at der er for meget skygge, slå fra om natten 
    }
}