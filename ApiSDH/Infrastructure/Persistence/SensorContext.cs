using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class SensorContext(DbContextOptions<SensorContext> options) : DbContext(options), ISensorContext // interface 
{
    public DbSet<SensorReading> SensorReadings { get; set; }

    // model builder 
}