using MediatR;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Queries.ObterPorId;

public class ObterSinistroPorIdHandler : IRequestHandler<ObterSinistroPorIdQuery, Resultado<SinistroDto>>
{
    private readonly ISinistroRepositorio _sinistroRepositorio;

    public ObterSinistroPorIdHandler(ISinistroRepositorio sinistroRepositorio)
    {
        _sinistroRepositorio = sinistroRepositorio;
    }

    public async Task<Resultado<SinistroDto>> Handle(ObterSinistroPorIdQuery request, CancellationToken cancellationToken)
    {
        var sinistro = await _sinistroRepositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (sinistro is null)
            return Resultado<SinistroDto>.Falha("Sinistro não encontrado.", TipoErro.NaoEncontrado);

        return Resultado<SinistroDto>.Sucesso(SinistroDto.Mapear(sinistro));
    }
}
