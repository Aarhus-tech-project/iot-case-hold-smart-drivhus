using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SensorContext(DbContextOptions<SensorContext> options) : DbContext(options), ISensorContext
{
    public DbSet<SensorReading> SensorReadings { get; set; }
    public DbSet<Config> Configs { get; set; }

    // model builder?
}