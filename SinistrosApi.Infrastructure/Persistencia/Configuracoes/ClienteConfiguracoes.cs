using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SinistrosApi.Domain.Entidades;

namespace SinistrosApi.Infrastructure.Persistencia.Configuracoes;

public class ClienteConfiguracoes : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.OwnsOne(c => c.Nome, nome =>
        {
            nome.Property(n => n.Nome)
                .HasColumnName("nome")
                .IsRequired()
                .HasMaxLength(150);

            nome.Property(n => n.Sobrenome)
                .HasColumnName("sobrenome")
                .IsRequired()
                .HasMaxLength(150);
        });

        builder.OwnsOne(c => c.Documento, documento =>
        {
            documento.Property(d => d.Tipo)
                .HasColumnName("tipo_documento")
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(4);

            documento.Property(d => d.Numero)
                .HasColumnName("numero_documento")
                .IsRequired()
                .HasMaxLength(14);

            documento.HasIndex(d => d.Numero).IsUnique();
        });

        builder.Property(c => c.DataReferencia).HasColumnName("data_referencia");
    }
}
