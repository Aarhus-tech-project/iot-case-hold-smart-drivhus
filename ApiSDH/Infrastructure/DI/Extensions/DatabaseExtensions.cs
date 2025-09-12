using Application.Common.Interfaces.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI.Extensions;

public static class DatabaseExtensions
{
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudMode = configuration.GetValue<bool>("CloudMode");

        var connectionString = "";

        if (cloudMode)
            connectionString =
                configuration.GetConnectionString("AzureConnection"); // read from enabled config             
        else
            connectionString = configuration.GetConnectionString("DefaultConnection"); // read from enabled config 

        if (connectionString is null) throw new Exception("Connection string is null");

        services.AddDbContext<SensorContext>((provider, options) => { options.UseSqlServer(connectionString); });

        services.AddScoped<ISensorContext>(provider => provider.GetRequiredService<SensorContext>());

        return services;
    }
}