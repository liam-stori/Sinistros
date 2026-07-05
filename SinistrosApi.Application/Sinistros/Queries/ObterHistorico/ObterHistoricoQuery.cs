using MediatR;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Queries.ObterHistorico;

public record ObterHistoricoQuery(Guid SinistroId) : IRequest<Resultado<IReadOnlyList<HistoricoSinistroDto>>>;
