using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;
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
    private DateTimeOffset latestHumiCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestLightCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestSoildMoistureCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestTempCheck = DateTimeOffset.UtcNow;
    private DateTimeOffset latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        statusService.Write("Status: CalculateActionService started");

        while (!cancellationToken.IsCancellationRequested)
        {
            /*
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();

            // Todo Move usage of status service into mqtt publisher and sms service

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
                    }
            }

            // check co2, open windown for fresh air
            if (CheckCo2(user, dataSet))
                if (DateTimeOffset.UtcNow - latestCo2Check > TimeSpan.FromMinutes(20))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Co2 low."), cancellationToken); // udluft
                    latestCo2Check = DateTimeOffset.UtcNow;
                }

            if (CheckTemp(user, dataSet))
                if (DateTimeOffset.UtcNow - latestTempCheck > TimeSpan.FromMinutes(20))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Temp low."), cancellationToken);
                    latestTempCheck = DateTimeOffset.UtcNow;
                }


            if (CheckHumi(user, dataSet))
                if (DateTimeOffset.UtcNow - latestHumiCheck > TimeSpan.FromMinutes(20))
                {
                    await mediator.Publish(new PreformActionEvent("Event: Humi low."), cancellationToken);
                    latestHumiCheck = DateTimeOffset.UtcNow;
                }


            if (CheckLightLevel(user, dataSet))
                if (DateTimeOffset.UtcNow - latestLightCheck > TimeSpan.FromMinutes(20))
                {
                    await smsService.SendSmsAsync(user.PhoneNumber, "Light level in green ouse is low.");
                    latestLightCheck = DateTimeOffset.UtcNow;
                }

            end:
            // Todo: Add way to close window again lol.
*/

            // await mediator.Publish(new PreformActionEvent("Event: Open window."));
            //
            // await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            //
            // await mediator.Publish(new PreformActionEvent("Event: Close window."));
            //
            // await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            // add startup sms, shows the app did susscessfuly launch and run this service, 
            //  await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
        }
    }


    // check if water tank is in need of more water 
    public bool CheckWater(Config config, List<SensorReading> dataSet)
    {
        if (config.WaterLimit >= dataSet.Average(sr => sr.WaterLevel)) return true;
        return false;
    }

    // Check if we need to water plant
    public bool CheckSoilMoisture(Config config, List<SensorReading> dataSet)
    {
        if (config.SoilMoistureLimit >= dataSet.Average(sr => sr.SoilHumidity)) return true;
        return false;
    }

    public bool CheckCo2(Config config, List<SensorReading> dataSet)
    {
        if (config.Co2Limit >= dataSet.Average(sr => sr.Co2)) return true;
        return false;
    }

    public bool CheckLightLevel(Config config, List<SensorReading> dataSet)
    {
        if (config.LightLimit >= dataSet.Average(sr => sr.Lux)) return true;
        return false; // send sms med at der er for meget skygge, slå fra om natten 
    }

    public bool CheckTemp(Config config, List<SensorReading> dataSet)
    {
        if (config.TempLimit <= dataSet.Average(sr => sr.Temp)) return true;
        return false;
    }

    public bool CheckHumi(Config config, List<SensorReading> dataSet)
    {
        if (config.HumiLimit >= dataSet.Average(sr => sr.Humidity)) return true;
        return false;
    }
}