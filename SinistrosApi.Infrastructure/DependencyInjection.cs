using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Infrastructure.Persistencia;
using SinistrosApi.Infrastructure.Persistencia.Repositorios;

namespace SinistrosApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<SinistrosApiDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(SinistrosApiDbContext).Assembly.GetName().Name))
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
        services.AddScoped<IApoliceRepositorio, ApoliceRepositorio>();
        services.AddScoped<ISinistroRepositorio, SinistroRepositorio>();

        return services;
    }

    public static async Task InicializarBancoAsync(this IServiceProvider services)
    {
        using var escopo = services.CreateScope();
        var context = escopo.ServiceProvider.GetRequiredService<SinistrosApiDbContext>();
        await CriarBancoSeNaoExistirAsync(context);
        await context.Database.EnsureCreatedAsync();
        await SemeadorDados.SemearAsync(context);
    }

    private static async Task CriarBancoSeNaoExistirAsync(SinistrosApiDbContext context)
    {
        var connectionString = context.Database.GetConnectionString()!;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var nomeBanco = builder.Database!;
        builder.Database = "postgres";

        await using var conn = new NpgsqlConnection(builder.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            $"SELECT 1 FROM pg_database WHERE datname = '{nomeBanco}'", conn);
        var existe = await cmd.ExecuteScalarAsync();

        if (existe is null)
        {
            await using var criar = new NpgsqlCommand($"CREATE DATABASE \"{nomeBanco}\"", conn);
            await criar.ExecuteNonQueryAsync();
        }
    }
}
