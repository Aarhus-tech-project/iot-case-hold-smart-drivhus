using Application.SensorReadings.Dtos;
using Application.Users.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SensorReading, SensorReadingDto>();
        CreateMap<UserInfo, UserDto>();
    }
}