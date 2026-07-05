using MediatR;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Application.Sinistros.Queries.Listar;

public record ListarSinistrosQuery(
    StatusSinistro? Status,
    DateTime? DataInicio,
    DateTime? DataFim,
    int Pagina = 1,
    int TamanhoPagina = 20) : IRequest<Resultado<SinistrosPaginadosDto>>;
