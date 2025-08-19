using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class InterfaceExtensions
{
    public static IServiceCollection AddInterfaceImplementations(this IServiceCollection services)
    {
        return services;
    }
}