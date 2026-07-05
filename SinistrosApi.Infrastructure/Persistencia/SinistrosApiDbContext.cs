using Microsoft.EntityFrameworkCore;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia;

public class SinistrosApiDbContext(DbContextOptions<SinistrosApiDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Apolice> Apolices { get; set; }
    public DbSet<Sinistro> Sinistros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SinistrosApiDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
