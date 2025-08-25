using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Factories.Entity;
using Application.Common.Interfaces.Services;
using Infrastructure.InterfaceImplementations.Factories.Entity;
using Infrastructure.InterfaceImplementations.Factories.Result;
using Infrastructure.InterfaceImplementations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IResultFactory, ResultFactory>();
        services.AddScoped<ISensorReadingFactory, SensorReadingFactory>();
        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}