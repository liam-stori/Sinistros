using MediatR;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;

public record AtualizarStatusCommand(
    Guid Id,
    StatusSinistro NovoStatus,
    string? Motivo,
    decimal? ValorAprovado) : IRequest<Resultado>;
