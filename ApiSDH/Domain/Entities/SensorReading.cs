using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     Entity for saving sensor data.
/// </summary>
public class SensorReading : BaseEntity
{
    public int Lux { get; set; }
    public int SoilHumidity { get; set; }
    public int Temp { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public int Co2 { get; set; }
    public int WaterLevel { get; set; }
}