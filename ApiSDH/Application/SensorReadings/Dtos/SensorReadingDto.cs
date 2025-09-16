namespace Application.SensorReadings.Dtos;

public class SensorReadingDto // TODO this map is incorrect
{
    public float Humidity { get; set; }
    public float Temperature { get; set; }
    public float Pressure { get; set; }


    public float DirtHumidity { get; set; }

    public float LightLevel { get; set; }
}

// Wrong reponse map examble
//{
// "humidity": 3,
// "temperature": 0,
// "pressure": 3,
// "dirtHumidity": 0,
// "lightLevel": 0
// }