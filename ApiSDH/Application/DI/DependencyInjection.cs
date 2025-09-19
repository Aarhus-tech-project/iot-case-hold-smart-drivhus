using Application.Common.Models;
using Application.SensorReadings.Commands.Create;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DI;

/// <summary>
///     DI for things related to the application layer.
/// </summary>
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