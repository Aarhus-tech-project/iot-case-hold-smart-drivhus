using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Persistence;

public interface ISensorContext
{
    DbSet<SensorReading> SensorReadings { get; }
}