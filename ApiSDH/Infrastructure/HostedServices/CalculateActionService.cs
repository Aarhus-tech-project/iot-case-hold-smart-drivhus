using Application.Common.Events;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.HostedServices;

public class CalculateActionService(
    IServiceScopeFactory scopeFactory,
    IMediator mediator,
    ISmsService smsService,
    IStatusService statusService)
    : BackgroundService
{
    private DateTimeOffset latestCo2Check = DateTimeOffset.UtcNow;
    private DateTimeOffset latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
    private DateTimeOffset latestSoildMoistureCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        statusService.Write("Status: CalculateActionService started");

        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();

            // TODO calculate action


            var user = await db.Users.FirstOrDefaultAsync(cancellationToken);

            if (user is null) // throws app error, user inforcer burde have kørt 
                throw new InvalidOperationException();

            // Dobble check at det er sorteret korrekt 

            var dataSet = await db.SensorReadings.OrderByDescending(sr => sr.CreatedAt).Take(20)
                .ToListAsync(cancellationToken);

            if (dataSet.Count < 6)
            {
                if (DateTimeOffset.UtcNow - latestDataSetLowSmsDate > TimeSpan.FromMinutes(20))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber,
                        "Dataset is low on data, api can not calculate action yet.");
                    latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
                    statusService.Write("Sending sms: Dataset is low on data, api can not calculate action yet.");
                }

                goto end;
            }


            // chain water and send sms if tank is low 
            if (CheckWater(user, dataSet))
            {
                if (DateTimeOffset.UtcNow - latestWaterTankLowSmsDate > TimeSpan.FromMinutes(20))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber, "Water level is low, please refill water tank.");
                    latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;
                    statusService.Write("Sending sms: Water level is low, please refill water tank.");
                }
            }
            else
            {
                if (CheckSoilMoisture(user,
                        dataSet)) // check if the plant required new water, only if the tank has water
                    if (DateTimeOffset.UtcNow - latestSoildMoistureCheck > TimeSpan.FromMinutes(20))
                    {
                        await mediator.Publish(new PreformActionEvent("Event: Soil Moisture low."), cancellationToken);
                        latestSoildMoistureCheck = DateTimeOffset.UtcNow;
                        statusService.Write("Status: Event: Soil Moisture low.");
                    }
            }

            // check co2, open windown for fresh air 
            if (CheckCo2(user, dataSet))
                if (DateTimeOffset.UtcNow - latestCo2Check > TimeSpan.FromMinutes(20))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Co2 low."), cancellationToken); // udluft 
                    latestCo2Check = DateTimeOffset.UtcNow;
                    statusService.Write("Status: Event: Co2 low.");
                }

            // check light level 


            //await mediator.Publish(new PreformActionEvent("parse data here"), cancellationToken);
            end: ;

            // add startup sms, shows the app did susscessfuly launch and run this service, 
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
        if (user.Co2Limit >= dataSet.Average(sr => sr.Co2)) return true;
        return false;
    }

    public bool CheckLightLevel(UserInfo user, List<SensorReading> dataSet)
    {
        return true; // send sms med at der er for meget skygge, slå fra om natten 
    }
}