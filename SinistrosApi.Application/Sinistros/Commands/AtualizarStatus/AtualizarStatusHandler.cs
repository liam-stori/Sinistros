using MediatR;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Domain.Comum;

namespace SinistrosApi.Application.Sinistros.Commands.AtualizarStatus;

public class AtualizarStatusHandler : IRequestHandler<AtualizarStatusCommand, Resultado>
{
    private readonly ISinistroRepositorio _sinistroRepositorio;
    private readonly TimeProvider _timeProvider;

    public AtualizarStatusHandler(ISinistroRepositorio sinistroRepositorio,
        TimeProvider timeProvider)
    {
        _sinistroRepositorio = sinistroRepositorio;
        _timeProvider = timeProvider;
    }

    public async Task<Resultado> Handle(AtualizarStatusCommand request, CancellationToken cancellationToken)
    {
        var sinistro = await _sinistroRepositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (sinistro is null)
            return Resultado.Falha("Sinistro não encontrado.", TipoErro.NaoEncontrado);

        var resultado = sinistro.AtualizarStatus(request.NovoStatus,
            request.Motivo,
            request.ValorAprovado,
            _timeProvider.GetUtcNow().DateTime);

        if (resultado.EhFalha)
            return resultado;

        await _sinistroRepositorio.SalvarAlteracoesAsync(cancellationToken);

        return Resultado.Sucesso();
    }
}
