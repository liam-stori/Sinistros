using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Application.Interfaces;

public interface IApoliceRepositorio
{
    Task<Apolice?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
}
