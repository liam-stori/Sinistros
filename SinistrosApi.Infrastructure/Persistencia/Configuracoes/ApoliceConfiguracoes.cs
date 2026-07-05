using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Configuracoes;

public class ApoliceConfiguracoes : IEntityTypeConfiguration<Apolice>
{
    public void Configure(EntityTypeBuilder<Apolice> builder)
    {
        builder.ToTable("apolices");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.NumeroApolice)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(a => a.NumeroApolice).IsUnique();

        builder.Property(a => a.ClienteId).IsRequired();

        builder.Property(a => a.Ramo)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.DataInicioVigencia).HasColumnType("date");

        builder.Property(a => a.DataFimVigencia).HasColumnType("date");

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
