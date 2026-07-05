using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Application.Interfaces;

public interface ISinistroRepositorio
{
    Task<Sinistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<HistoricoSinistro>?> ObterHistoricoAsync(Guid sinistroId, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Sinistro> Itens, int Total)> ListarAsync(
        StatusSinistro? status,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken);
    Task AdicionarAsync(Sinistro sinistro, CancellationToken cancellationToken);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken);
}
