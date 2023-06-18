using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SecretsSharingTool.Core.Interfaces;
using SecretsSharingTool.Core.Pipelines;

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
}