using MediatR;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Queries.ObterHistorico;

public class ObterHistoricoHandler : IRequestHandler<ObterHistoricoQuery, Resultado<IReadOnlyList<HistoricoSinistroDto>>>
{
    private readonly ISinistroRepositorio _sinistroRepositorio;

    public ObterHistoricoHandler(ISinistroRepositorio sinistroRepositorio)
    {
        _sinistroRepositorio = sinistroRepositorio;
    }

    public async Task<Resultado<IReadOnlyList<HistoricoSinistroDto>>> Handle(ObterHistoricoQuery request,
        CancellationToken cancellationToken)
    {
        var historico = await _sinistroRepositorio.ObterHistoricoAsync(request.SinistroId, cancellationToken);
        if (historico is null)
            return Resultado<IReadOnlyList<HistoricoSinistroDto>>.Falha("Sinistro não encontrado.", TipoErro.NaoEncontrado);

        var dto = historico
            .Select(HistoricoSinistroDto.Mapear)
            .ToList();

        return Resultado<IReadOnlyList<HistoricoSinistroDto>>.Sucesso(dto);
    }
}
