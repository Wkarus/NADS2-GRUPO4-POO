using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servidor_PI.Models;

namespace Servidor_PI.Data.Maps
{
    public class CampanhaMap : IEntityTypeConfiguration<Campanha>
    {
        public void Configure(EntityTypeBuilder<Campanha> builder)
        {
            // Tabela campanha
            builder.ToTable("Campanha");

            // Chave primaria
            builder.HasKey(c => c.cd_campanha);

            builder.Property(c => c.cd_campanha)
                .ValueGeneratedOnAdd();

            // Campos principais
            builder.Property(c => c.nome_campanha)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.meta_arrecadacao)
                .HasColumnType("REAL");

            builder.Property(c => c.inicio)
                .HasColumnType("DATE");

            builder.Property(c => c.fim)
                .HasColumnType("DATE");

            // Relacionamentos
            builder.HasMany(c => c.Doacoes)
                .WithOne(d => d.Campanha)
                .HasForeignKey(d => d.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Noticias)
                .WithOne(n => n.Campanha)
                .HasForeignKey(n => n.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Relatorios)
                .WithOne(r => r.Campanha)
                .HasForeignKey(r => r.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
