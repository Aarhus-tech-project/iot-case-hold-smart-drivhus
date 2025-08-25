using Application.Common.Interfaces.Factories.Entity;
using Application.SensorReadings.Commands.Create;
using Domain.Entities;

namespace Infrastructure.InterfaceImplementations.Factories.Entity;

public class SensorReadingFactory : ISensorReadingFactory
{
    public SensorReading Create(CreateSensorReadingCommand command)
    {
        return new SensorReading
        {
            Humidity = command.Humidity,
            Temperature = command.Temperature,
            Pressure = command.Pressure,
            DirtHumidity = command.DirtHumidity,
            LightLevel = command.LightLevel
        };
    }
}