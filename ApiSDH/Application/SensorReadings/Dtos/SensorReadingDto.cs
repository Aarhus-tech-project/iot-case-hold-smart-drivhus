namespace Application.SensorReadings.Dtos;

public class SensorReadingDto
{
    public int Lux { get; set; }
    public int SoilHumidity { get; set; }
    public int Temp { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public int Co2 { get; set; }
    public int WaterLevel { get; set; }
}