using Infrastructure.Common.Services;
using Infrastructure.DI.Extensions;
using Infrastructure.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.DI;

/// <summary>
///     DI for infrastructure related things.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        IHostBuilder builder)
    {
        services.AddDatabase(configuration);

        services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));

        services.AddInterfaceImplementations();

        services.AddHostedService<CalculateActionService>();

        services.AddCustomSerilog(configuration, builder); // Pretty late to add logging.

        return services;
    }
}