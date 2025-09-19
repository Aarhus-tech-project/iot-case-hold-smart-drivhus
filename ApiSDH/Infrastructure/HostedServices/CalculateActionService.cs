using Application.Common.Events;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.HostedServices;

/// <summary>
///     <see cref="CalculateActionService" /> is responsible for controlling the greenhouse. It monitors sensor data and
///     takes action if appropriate.
/// </summary>
public class CalculateActionService(
    IServiceScopeFactory scopeFactory,
    IMediator mediator,
    ILogger<CalculateActionService> logger,
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
        try
        {
            statusService.Write("Status: CalculateActionService started");

            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ISensorContext>();

                var config = await db.Configs.FirstOrDefaultAsync(cancellationToken);

                if (config is null) throw new InvalidOperationException();

                var dataSet = await db.SensorReadings.OrderByDescending(sr => sr.CreatedAt).Take(20)
                    .ToListAsync(cancellationToken);

                if (dataSet.Count < 20)
                {
                    if (DateTimeOffset.UtcNow - latestDataSetLowSmsDate > TimeSpan.FromMinutes(20))
                    {
                        await smsService.SendSmsAsync(config.PhoneNumber,
                            "Dataset is low on data, api can not calculate action yet.");
                        latestDataSetLowSmsDate = DateTimeOffset.UtcNow;
                    }

                    goto end;
                }

                // chain water and send sms if tank is low
                if (CheckWater(config, dataSet))
                {
                    if (DateTimeOffset.UtcNow - latestWaterTankLowSmsDate > TimeSpan.FromMinutes(20))
                    {
                        await smsService.SendSmsAsync(config.PhoneNumber,
                            "Water level is low, please refill water tank.");
                        latestWaterTankLowSmsDate = DateTimeOffset.UtcNow;
                    }
                }
                else
                {
                    if (CheckSoilMoisture(config,
                            dataSet)) // check if the plant required new water, only if the tank has water
                        if (DateTimeOffset.UtcNow - latestSoildMoistureCheck > TimeSpan.FromMinutes(20))
                        {
                            await mediator.Publish(new PreformActionEvent("Event: Soil Moisture low."),
                                cancellationToken);
                            latestSoildMoistureCheck = DateTimeOffset.UtcNow;
                        }
                }

                // check co2, open windown for fresh air
                if (CheckCo2(config, dataSet))
                    if (DateTimeOffset.UtcNow - latestCo2Check > TimeSpan.FromMinutes(20))
                    {
                        await mediator.Publish(new PreformActionEvent("Event: Co2 low."), cancellationToken); // udluft
                        latestCo2Check = DateTimeOffset.UtcNow;
                    }

                if (CheckTemp(config, dataSet))
                    if (DateTimeOffset.UtcNow - latestTempCheck > TimeSpan.FromMinutes(20))
                    {
                        await mediator.Publish(new PreformActionEvent("Event: Temp too high."), cancellationToken);
                        latestTempCheck = DateTimeOffset.UtcNow;
                    }


                if (CheckHumi(config, dataSet))
                    if (DateTimeOffset.UtcNow - latestHumiCheck > TimeSpan.FromMinutes(20))
                    {
                        await mediator.Publish(new PreformActionEvent("Event: Humi too high."), cancellationToken);
                        latestHumiCheck = DateTimeOffset.UtcNow;
                    }


                if (CheckLightLevel(config, dataSet))
                    if (DateTimeOffset.UtcNow - latestLightCheck > TimeSpan.FromMinutes(20))
                    {
                        await smsService.SendSmsAsync(config.PhoneNumber, "Light level in green ouse is low.");
                        latestLightCheck = DateTimeOffset.UtcNow;
                    }

                end: ;
                // Todo: Add way to close window again lol.

                await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message + "Error in CalculateActionService.");
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
        // Tag højde for om natten. 
        if (config.LightLimit >= dataSet.Average(sr => sr.Lux)) return true;
        return false;
    }

    public bool CheckTemp(Config config, List<SensorReading> dataSet)
    {
        if (config.TempLimit <= dataSet.Average(sr => sr.Temp)) return true;
        return false;
    }

    public bool CheckHumi(Config config, List<SensorReading> dataSet)
    {
        if (config.HumiLimit <= dataSet.Average(sr => sr.Humidity)) return true;
        return false;
    }
}