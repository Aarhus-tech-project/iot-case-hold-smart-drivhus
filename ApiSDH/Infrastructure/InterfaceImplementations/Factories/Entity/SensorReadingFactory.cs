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
                Lux = command.lux,
                SoilHumidity = command.soilhumidity,
                Temp = command.temp,
                Pressure = command.pressure,
                Humidity = command.humidity,
                Co2 = command.co2,
                WaterLevel = command.waterlevel
            };
        }
    }