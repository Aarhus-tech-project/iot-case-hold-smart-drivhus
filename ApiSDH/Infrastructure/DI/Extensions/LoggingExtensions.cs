using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.DI.Extensions;

public static class LoggingExtensions
{
    public static (IServiceCollection Services, IHostBuilder HostBuilder) AddCustomSerilog(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, servicesProvider, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(servicesProvider)
                .Enrich.FromLogContext());

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        return (services, hostBuilder);
    }
}