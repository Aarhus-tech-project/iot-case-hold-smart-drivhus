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

// {
/*
"Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
        "Default": "Information",
        "Override": {
            "Microsoft": "Warning",
            "System": "Warning"
        }
    },
    "WriteTo": [
    {
        "Name": "Console",
        "Args": {
            "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
        }
    },
    {
        "Name": "File",
        "Args": {
            "path": "Logs/log-.txt",
            "rollingInterval": "Day",
            "retainedFileCountLimit": 14,
            "restrictedToMinimumLevel": "Information",
            "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
        }
    }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
        "Application": "ApiSDH"
    }
}
}
*/