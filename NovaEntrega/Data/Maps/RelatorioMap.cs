using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servidor_PI.Enums;
using Servidor_PI.Models;

namespace Servidor_PI.Data.Maps
{
    public class RelatorioMap : IEntityTypeConfiguration<Relatorio>
    {
        public void Configure(EntityTypeBuilder<Relatorio> builder)
        {
            // Tabela relatorio
            builder.ToTable("Relatorio");

            // Chave primaria
            builder.HasKey(r => r.cd_relatorio);

            builder.Property(r => r.cd_relatorio)
                .ValueGeneratedOnAdd();

            // Relacionamento com campanha
            builder.Property(r => r.cd_campanha)
                .IsRequired();

            // Tipo do relatorio (enum como string)
            builder.Property(r => r.tipo_relatorio)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            // Valores e datas
            builder.Property(r => r.valor_gasto)
                .HasColumnType("REAL");

            builder.Property(r => r.data_relatorio)
                .IsRequired()
                .HasColumnType("DATE");

            // Relacionamento: relatorio pertence a uma campanha
            builder.HasOne(r => r.Campanha)
                .WithMany(c => c.Relatorios)
                .HasForeignKey(r => r.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
