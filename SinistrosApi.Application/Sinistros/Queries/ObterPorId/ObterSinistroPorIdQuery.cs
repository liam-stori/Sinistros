using MediatR;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Queries.ObterPorId;

public record ObterSinistroPorIdQuery(Guid Id) : IRequest<Resultado<SinistroDto>>;
