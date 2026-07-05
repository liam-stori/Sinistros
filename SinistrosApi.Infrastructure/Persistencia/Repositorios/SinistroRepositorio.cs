using Microsoft.EntityFrameworkCore;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Domain.Entidades;
using SinistrosApi.Domain.Enumeradores;

namespace SinistrosApi.Infrastructure.Persistencia.Repositorios;

public class SinistroRepositorio : ISinistroRepositorio
{
    private readonly SinistrosApiDbContext _context;

    public SinistroRepositorio(SinistrosApiDbContext context)
    {
        _context = context;
    }

    public async Task<Sinistro?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sinistros
            .Include(s => s.Historico)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Sinistro> Itens, int Total)> ListarAsync(
        StatusSinistro? status,
        DateTime? dataInicio,
        DateTime? dataFim,
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken)
    {
        var query = _context.Sinistros.AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        if (dataInicio.HasValue)
            query = query.Where(s => s.DataAbertura >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(s => s.DataAbertura <= dataFim.Value.Date.AddDays(1).AddTicks(-1));

        var total = await query.CountAsync(cancellationToken);

        var itens = await query
            .OrderByDescending(s => s.DataAbertura)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (itens, total);
    }

    public async Task<IReadOnlyList<HistoricoSinistro>?> ObterHistoricoAsync(Guid sinistroId, CancellationToken cancellationToken)
    {
        var historico = await _context.Set<HistoricoSinistro>()
            .Where(h => h.SinistroId == sinistroId)
            .OrderBy(h => h.DataAlteracao)
            .ToListAsync(cancellationToken);

        return historico.Count == 0 ? null : historico;
    }

    public Task AdicionarAsync(Sinistro sinistro, CancellationToken cancellationToken)
    {
        _context.Sinistros.Add(sinistro);
        return Task.CompletedTask;
    }

    public async Task SalvarAlteracoesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
