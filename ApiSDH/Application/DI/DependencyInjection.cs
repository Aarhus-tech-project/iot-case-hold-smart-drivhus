using Application.Common.Models;
using Application.SensorReadings.Commands.Create;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateSensorReadingCommand>();

        services.AddAutoMapper(cfg => { },
            typeof(MappingProfile).Assembly
        );

        return services;
    }
}