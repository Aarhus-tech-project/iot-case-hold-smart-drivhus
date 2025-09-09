using Domain.Common;

namespace Domain.Entities;

public class UserInfo : BaseEntity
{
    public string PhoneNumber { get; set; } = "+4560592406";


    public int WaterLimit { get; set; } = 100; // midnste verdi 
    public int SoilMoistureLimit { get; set; } = 100; // midnste verdi 
    public int Co2Limit { get; set; } = 100; //  midnste verdi

}