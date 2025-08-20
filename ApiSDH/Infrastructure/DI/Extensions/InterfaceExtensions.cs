using Application.Common.Interfaces.Factories;
using Infrastructure.InterfaceImplementations.Factories.Result;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        services.AddScoped<IResultFactory, ResultFactory>();

        return services;
    }
}