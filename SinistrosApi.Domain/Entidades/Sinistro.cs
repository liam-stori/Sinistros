using System.Diagnostics.CodeAnalysis;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Domain.Entidades;

public class Sinistro : EntidadeBase
{
    private readonly List<HistoricoSinistro> _historico = [];

    public Guid ApoliceId { get; private set; }
    public DateTime DataAbertura { get; private set; }
    public StatusSinistro Status { get; private set; }
    public decimal ValorEstimado { get; private set; }
    public decimal? ValorAprovado { get; private set; }
    public string? MotivoNegativa { get; private set => field = value?.Trim(); }

    public IReadOnlyCollection<HistoricoSinistro> Historico => _historico.AsReadOnly();

    [ExcludeFromCodeCoverage]
    protected Sinistro() { }

    private Sinistro(Guid apoliceId, decimal valorEstimado, DateTime dataAbertura)
    {
        ApoliceId = apoliceId;
        DataAbertura = dataAbertura;
        Status = StatusSinistro.Aberto;
        ValorEstimado = valorEstimado;
    }

    public static Resultado<Sinistro> Abrir(Apolice apolice, decimal valorEstimado, DateTime dataAbertura)
    {
        if (!apolice.EstaVigenteEAtiva(DateOnly.FromDateTime(dataAbertura)))
            return Resultado<Sinistro>.Falha("Sinistro só pode ser aberto em apólice com status Ativa.", TipoErro.RegraDeNegocio);

        if (valorEstimado <= 0)
            return Resultado<Sinistro>.Falha("Valor estimado deve ser maior que zero.", TipoErro.RegraDeNegocio);

        var sinistro = new Sinistro(apolice.Id, valorEstimado, dataAbertura);

        sinistro.RegistrarHistorico(null, StatusSinistro.Aberto, null, dataAbertura);

        return Resultado<Sinistro>.Sucesso(sinistro);
    }

    public Resultado AtualizarStatus(StatusSinistro novoStatus, string? motivo, decimal? valorAprovado, DateTime dataAlteracao)
    {
        if (!TransicaoValida(novoStatus))
            return Resultado.Falha($"Transição de {Status.ObterDescricao()} para {novoStatus.ObterDescricao()} não é permitida.", TipoErro.RegraDeNegocio);

        var validacao = ValidarInvariantes(novoStatus, motivo, valorAprovado);
        if (validacao.EhFalha) return validacao;

        var statusAnterior = Status;
        AplicarTransicao(novoStatus, motivo, valorAprovado);
        RegistrarHistorico(statusAnterior, novoStatus, motivo, dataAlteracao);

        return Resultado.Sucesso();
    }

    private static Resultado ValidarInvariantes(StatusSinistro novoStatus, string? motivo, decimal? valorAprovado)
    {
        return novoStatus switch
        {
            StatusSinistro.Negado => ValidarNegado(motivo),
            StatusSinistro.Encerrado => ValidarEncerrado(valorAprovado),
            _ => Resultado.Sucesso()
        };
    }

    private static Resultado ValidarNegado(string? motivo)
    {
        return string.IsNullOrWhiteSpace(motivo)
            ? Resultado.Falha("Motivo é obrigatório ao negar um sinistro.", TipoErro.RegraDeNegocio)
            : Resultado.Sucesso();
    }

    private static Resultado ValidarEncerrado(decimal? valorAprovado)
    {
        if (valorAprovado is null)
            return Resultado.Falha("Valor aprovado é obrigatório ao encerrar um sinistro.", TipoErro.RegraDeNegocio);

        if (valorAprovado <= 0)
            return Resultado.Falha("Valor aprovado deve ser maior que zero.", TipoErro.RegraDeNegocio);

        return Resultado.Sucesso();
    }

    private void AplicarTransicao(StatusSinistro novoStatus, string? motivo, decimal? valorAprovado)
    {
        Status = novoStatus;

        switch (novoStatus)
        {
            case StatusSinistro.Negado:
                MotivoNegativa = motivo;
                break;
            case StatusSinistro.Encerrado:
                ValorAprovado = valorAprovado;
                break;
        }
    }

    private void RegistrarHistorico(StatusSinistro? statusAnterior, StatusSinistro statusNovo, string? motivo, DateTime data)
    {
        _historico.Add(HistoricoSinistro.Criar(Id, statusAnterior, statusNovo, motivo, data));
    }

    private bool TransicaoValida(StatusSinistro novoStatus) => (Status, novoStatus) switch
    {
        (StatusSinistro.Aberto, StatusSinistro.EmAnalise) or
        (StatusSinistro.EmAnalise, StatusSinistro.Aprovado) or
        (StatusSinistro.EmAnalise, StatusSinistro.Negado) or
        (StatusSinistro.Aprovado, StatusSinistro.Encerrado) => true,
        _ => false
    };
}
