using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Application.Interfaces;

public interface IClienteRepositorio
{
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
}
