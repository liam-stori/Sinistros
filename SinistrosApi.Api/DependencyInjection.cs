using System.Diagnostics.CodeAnalysis;

namespace SinistrosApi.Api;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AdicionarApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
