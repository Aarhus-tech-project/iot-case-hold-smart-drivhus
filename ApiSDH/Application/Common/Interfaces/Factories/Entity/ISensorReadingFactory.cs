using Application.SensorReadings.Commands.Create;
using Domain.Entities;

namespace Application.Common.Interfaces.Factories.Entity;

/// <summary>
///     Application Interfaces have their implementations in the infrastructure layer.
///     <see cref="ISensorReadingFactory" /> is used to create instances of
///     <see cref="SensorReading" />.
/// </summary>
public interface ISensorReadingFactory
{
    public SensorReading Create(CreateSensorReadingCommand command);
}