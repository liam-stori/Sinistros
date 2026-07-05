using SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Api.Requesters;

public record AtualizarStatusRequest(string? NovoStatus, string? Motivo, decimal? ValorAprovado)
{
    public AtualizarStatusCommand ToCommand(Guid id) =>
        new(id,
            Enum.TryParse<StatusSinistro>(NovoStatus, out var status) ? status : (StatusSinistro)0,
            Motivo,
            ValorAprovado);
}
