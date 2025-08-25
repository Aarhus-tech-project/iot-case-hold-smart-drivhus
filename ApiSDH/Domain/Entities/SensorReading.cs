using Domain.Common;

namespace Domain.Entities;

public class SensorReading : BaseEntity
{
    public float Humidity { get; set; }
}