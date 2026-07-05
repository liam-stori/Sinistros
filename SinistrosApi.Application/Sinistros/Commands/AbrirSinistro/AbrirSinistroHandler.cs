using MediatR;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Application.Sinistros.Dtos;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Application.Sinistros.Commands.AbrirSinistro;

public class AbrirSinistroHandler : IRequestHandler<AbrirSinistroCommand, Resultado<SinistroDto>>
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly IApoliceRepositorio _apoliceRepositorio;
    private readonly TimeProvider _timeProvider;

    public AbrirSinistroHandler(ISinistroRepositorio sinistroRepositorio,
        IApoliceRepositorio apoliceRepositorio,
        TimeProvider timeProvider)
    {
        _sinistroRepositorio = sinistroRepositorio;
        _apoliceRepositorio = apoliceRepositorio;
        _timeProvider = timeProvider;
    }

    public async Task<Resultado<SinistroDto>> Handle(AbrirSinistroCommand request, CancellationToken cancellationToken)
    {
        var apolice = await _apoliceRepositorio.ObterPorIdAsync(request.ApoliceId, cancellationToken);
        if (apolice is null)
            return Resultado<SinistroDto>.Falha("Apólice não encontrada.", TipoErro.NaoEncontrado);

        var resultado = Sinistro.Abrir(apolice, request.ValorEstimado, _timeProvider.GetUtcNow().DateTime);
        if (resultado.EhFalha)
            return Resultado<SinistroDto>.Falha(resultado.Erro!, resultado.TipoErro);

        await _sinistroRepositorio.AdicionarAsync(resultado.Valor, cancellationToken);
        await _sinistroRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return Resultado<SinistroDto>.Sucesso(SinistroDto.Mapear(resultado.Valor));
    }
}
