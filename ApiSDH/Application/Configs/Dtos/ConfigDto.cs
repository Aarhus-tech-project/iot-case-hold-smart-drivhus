namespace Application.Configs.Dtos;

public class ConfigDto
{
    public string PhoneNumber { get; set; }
    public int WaterLimit { get; set; }
    public int SoilMoistureLimit { get; set; }
    public int Co2Limit { get; set; }
    public int TempLimit { get; set; }
    public int HumiLimit { get; set; }
    public int LightLimit { get; set; }
}