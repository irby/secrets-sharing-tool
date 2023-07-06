using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SecretsSharingTool.Core.Interfaces;
using SecretsSharingTool.Core.Pipelines;
using SecretsSharingTool.Core.Providers;

namespace SecretsSharingTool.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        var assembly = typeof(ISecretsSharingToolCore).Assembly;
        services.AddMediatR(assembly);
        AssemblyScanner.FindValidatorsInAssembly(assembly)
            .ForEach(item => 
                services.AddScoped(item.InterfaceType, item.ValidatorType));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        return services;
    }

    public static IServiceCollection AddDateTimeProvider(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }
}