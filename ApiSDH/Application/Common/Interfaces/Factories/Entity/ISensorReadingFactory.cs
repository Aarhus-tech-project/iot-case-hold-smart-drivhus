using Application.SensorReadings.Commands.Create;
using Domain.Entities;

namespace Application.Common.Interfaces.Factories.Entity;

public interface ISensorReadingFactory
{
    public SensorReading Create(CreateSensorReadingCommand command);
}