using Domain.Common;

namespace Domain.Entities;

public class SensorReading : BaseEntity
{
    public string Humidity { get; set; }
    public string Temperature { get; set; }
    public string Pressure { get; set; }


    public string DirtHumidity { get; set; }

    public string LightLevel { get; set; }

    public int WaterLimit { get; set; }
}