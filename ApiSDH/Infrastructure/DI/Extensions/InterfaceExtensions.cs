using Application.Common.Interfaces.Factories;
using Application.Common.Interfaces.Factories.Entity;
using Application.Common.Interfaces.Services;
using Infrastructure.Common.Factories.Entity;
using Infrastructure.Common.Factories.Result;
using Infrastructure.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IResultFactory, ResultFactory>();
        services.AddScoped<ISensorReadingFactory, SensorReadingFactory>();

        services
            .AddTransient<ISmsService,
                SmsService>(); // Would perfer scoped but as this is used inside hosted service it obvi can not be scoped. 
        services.AddTransient<IStatusService, StatusService>();

        return services;
    }
}