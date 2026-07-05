using SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;

namespace SinistrosApi.Api.Requesters;

public record AbrirSinistroRequest(string? ApoliceId, decimal? ValorEstimado)
{
    public AbrirSinistroCommand ToCommand() =>
        new(Guid.TryParse(ApoliceId, out var id) ? id : Guid.Empty,
            ValorEstimado ?? 0);
}
