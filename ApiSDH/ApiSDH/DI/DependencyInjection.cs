using ApiSDH.Common.Interfaces.Factories;
using ApiSDH.Common.Interfaces.Services;
using ApiSDH.Common.Services;
using ApiSDH.Common.Services.Factories;
using Application.Common.Behaviors;
using MediatR;

namespace ApiSDH.DI;

public static class DependencyInjection
{
    /// <summary>
    ///     Sets up DI for things related to the presentation layer. <see cref="ApiSDH.DI.DependencyInjection" /> is used by
    ///     <see cref="Program" />.
    /// </summary>
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IResponseFactory, ResponseFactory>();

        services.AddSingleton<IIoTHubPublisherService, IoTHubPublisherService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Application.DI.DependencyInjection).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        // Application layer related. However, the app layer is registered in DI before the presentation DI is registered.
        // But CommandLifecycleBehavior must be registerd after MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandLifecycleBehavior<,>));

        return services;
    }
}