using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SinistrosApi.Application.Comportamentais;
using System.Diagnostics.CodeAnalysis;

namespace SinistrosApi.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AdicionarApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
