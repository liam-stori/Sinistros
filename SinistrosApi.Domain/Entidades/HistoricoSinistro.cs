using System.Diagnostics.CodeAnalysis;
using SinistrosApi.Domain.Comum;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Domain.Entidades;

public class HistoricoSinistro : EntidadeBase
{
    public Guid SinistroId { get; private set; }
    public StatusSinistro? StatusAnterior { get; private set; }
    public StatusSinistro StatusNovo { get; private set; }
    public DateTime DataAlteracao { get; private set; }
    public string? Motivo { get; private set => field = value?.Trim(); }

    [ExcludeFromCodeCoverage]
    protected HistoricoSinistro() { }

    private HistoricoSinistro(Guid sinistroId, StatusSinistro? statusAnterior, StatusSinistro statusNovo, string? motivo, DateTime dataAlteracao)
    {
        SinistroId = sinistroId;
        StatusAnterior = statusAnterior;
        StatusNovo = statusNovo;
        DataAlteracao = dataAlteracao;
        Motivo = motivo;
    }

    public static HistoricoSinistro Criar(Guid sinistroId, StatusSinistro? statusAnterior, StatusSinistro statusNovo, string? motivo, DateTime dataAlteracao)
        => new(sinistroId, statusAnterior, statusNovo, motivo, dataAlteracao);
}
