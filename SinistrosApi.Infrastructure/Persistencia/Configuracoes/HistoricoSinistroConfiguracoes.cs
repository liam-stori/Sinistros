using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Configuracoes;

public class HistoricoSinistroConfiguracoes : IEntityTypeConfiguration<HistoricoSinistro>
{
    public void Configure(EntityTypeBuilder<HistoricoSinistro> builder)
    {
        builder.ToTable("historico_sinistros");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.StatusAnterior)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(h => h.StatusNovo)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(h => h.Motivo).HasMaxLength(500);
    }
}
