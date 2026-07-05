using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Application.Sinistros.Dtos;

public record SinistroDto(
    Guid Id,
    Guid ApoliceId,
    DateTime DataAbertura,
    string Status,
    decimal ValorEstimado,
    decimal? ValorAprovado,
    string? MotivoNegativa)
{
    public static SinistroDto Mapear(Sinistro sinistro) => new(
        sinistro.Id,
        sinistro.ApoliceId,
        sinistro.DataAbertura,
        sinistro.Status.ObterDescricao(),
        sinistro.ValorEstimado,
        sinistro.ValorAprovado,
        sinistro.MotivoNegativa);
}
