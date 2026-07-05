using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Application.Sinistros.Dtos;

public record HistoricoSinistroDto(
    Guid Id,
    string? StatusAnterior,
    string StatusNovo,
    DateTime DataAlteracao,
    string? Motivo)
{
    public static HistoricoSinistroDto Mapear(HistoricoSinistro h) => new(
        h.Id,
        h.StatusAnterior?.ObterDescricao(),
        h.StatusNovo.ObterDescricao(),
        h.DataAlteracao,
        h.Motivo);
}
