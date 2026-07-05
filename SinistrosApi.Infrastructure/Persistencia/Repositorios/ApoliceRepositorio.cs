using Microsoft.EntityFrameworkCore;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Repositorios;

public class ApoliceRepositorio : IApoliceRepositorio
{
    private readonly SinistrosApiDbContext _context;

    public ApoliceRepositorio(SinistrosApiDbContext context)
    {
        _context = context;
    }

    public async Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Apolices.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }
}
