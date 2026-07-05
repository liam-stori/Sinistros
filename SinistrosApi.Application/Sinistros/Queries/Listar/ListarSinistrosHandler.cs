using MediatR;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Queries.Listar;

public class ListarSinistrosHandler : IRequestHandler<ListarSinistrosQuery, Resultado<SinistrosPaginadosDto>>
{
    private readonly ISinistroRepositorio _sinistroRepositorio;

    public ListarSinistrosHandler(ISinistroRepositorio sinistroRepositorio)
    {
        _sinistroRepositorio = sinistroRepositorio;
    }

    public async Task<Resultado<SinistrosPaginadosDto>> Handle(ListarSinistrosQuery request, CancellationToken cancellationToken)
    {
        var (itens, total) = await _sinistroRepositorio.ListarAsync(
            request.Status,
            request.DataInicio,
            request.DataFim,
            request.Pagina,
            request.TamanhoPagina,
            cancellationToken);

        var dto = new SinistrosPaginadosDto(
            itens.Select(SinistroDto.Mapear).ToList(),
            total,
            request.Pagina,
            request.TamanhoPagina);

        return Resultado<SinistrosPaginadosDto>.Sucesso(dto);
    }
}
