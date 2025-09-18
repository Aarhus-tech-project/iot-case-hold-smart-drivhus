using Domain.Common;

namespace Domain.Entities;

public class Config : BaseEntity
{
    public string PhoneNumber { get; set; } = "+4560592406";
    public int WaterLimit { get; set; } = 100;
    public int SoilMoistureLimit { get; set; } = 100;
    public int Co2Limit { get; set; } = 100;
    public int TempLimit { get; set; } = 100;
    public int HumiLimit { get; set; } = 100;
    public int LightLimit { get; set; } = 100;
}