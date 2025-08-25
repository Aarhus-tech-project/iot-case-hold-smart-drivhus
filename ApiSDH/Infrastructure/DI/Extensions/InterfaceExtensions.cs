using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Factories.Entity;
using Infrastructure.InterfaceImplementations.Factories.Entity;
using Infrastructure.InterfaceImplementations.Factories.Result;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IResultFactory, ResultFactory>();
        services.AddScoped<ISensorReadingFactory, SensorReadingFactory>();

        return services;
    }
}