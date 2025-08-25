using ApiSDH.Common.Interfaces.Factories;
using ApiSDH.Common.Interfaces.Services;
using ApiSDH.Common.Services;
using ApiSDH.Common.Services.Factories;

namespace ApiSDH.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IResponseFactory, ResponseFactory>();

        services.AddSingleton<IMqttPublisherService, MqttPublisherService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.DI.DependencyInjection).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        return services;
    }
}