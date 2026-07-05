using MediatR;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;

public record AbrirSinistroCommand(
    Guid ApoliceId,
    decimal ValorEstimado) : IRequest<Resultado<SinistroDto>>;
