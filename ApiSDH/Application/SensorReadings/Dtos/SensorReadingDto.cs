namespace Application.SensorReadings.Dtos;

public class SensorReadingDto
{
    public float Humidity { get; set; }
    public float Temperature { get; set; }
    public float Pressure { get; set; }


    public float DirtHumidity { get; set; }

    public float LightLevel { get; set; }
}