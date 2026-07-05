using Microsoft.EntityFrameworkCore;
using SinistrosApi.Application.Interfaces;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Repositorios;

public class ClienteRepositorio : IClienteRepositorio
{
    private readonly SinistrosApiDbContext _context;

    public ClienteRepositorio(SinistrosApiDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
