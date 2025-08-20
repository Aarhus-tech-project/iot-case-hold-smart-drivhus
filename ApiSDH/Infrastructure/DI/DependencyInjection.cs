using Infrastructure.DI.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        IHostBuilder builder)
    {
        services.AddDatabase(configuration);

        services.AddInterfaceImplementations();

        services.AddCustomSerilog(configuration, builder);

        return services;
    }
}