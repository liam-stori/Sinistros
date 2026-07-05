using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Configuracoes;

public class SinistroConfiguracoes : IEntityTypeConfiguration<Sinistro>
{
    public void Configure(EntityTypeBuilder<Sinistro> builder)
    {
        builder.ToTable("sinistros");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.ApoliceId).IsRequired();

        builder.Property(s => s.DataAbertura).IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.ValorEstimado)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.ValorAprovado).HasColumnType("numeric(18,2)");

        builder.Property(s => s.MotivoNegativa).HasMaxLength(500);

        builder.HasOne<Apolice>()
            .WithMany()
            .HasForeignKey(s => s.ApoliceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Historico)
            .WithOne()
            .HasForeignKey(h => h.SinistroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Historico)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_historico");
    }
}
