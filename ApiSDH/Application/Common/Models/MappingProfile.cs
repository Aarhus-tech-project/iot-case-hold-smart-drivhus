using Application.SensorReadings.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SensorReading, SensorReadingDto>();
    }
}