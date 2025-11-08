using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servidor_PI.Enums;
using Servidor_PI.Models;

namespace Servidor_PI.Data.Maps
{
    public class DoacaoMap : IEntityTypeConfiguration<Doacao>
    {
        public void Configure(EntityTypeBuilder<Doacao> builder)
        {
            // Tabela doacao (nome da tabela no banco)
            builder.ToTable("Doacao");

            // Chave primaria
            builder.HasKey(d => d.cd_doacao);

            builder.Property(d => d.cd_doacao)
                .ValueGeneratedOnAdd();

            // Campos obrigatorios de relacionamento
            builder.Property(d => d.cd_cliente)
                .IsRequired();

            builder.Property(d => d.cd_campanha)
                .IsRequired();

            // Nome/descricao da doacao
            builder.Property(d => d.nome_doacao)
                .IsRequired()
                .HasMaxLength(200);

            // Converte enums para string no banco (legivel)
            builder.Property(d => d.tipo_doacao)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(d => d.forma_arrecadacao)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(d => d.status_arrecadacao)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indice para buscas por status
            builder.HasIndex(d => d.status_arrecadacao)
                .HasDatabaseName("idx_doacao_status");

            // Relacionamento com Usuario (quem doou)
            builder.HasOne(d => d.Usuario)
                .WithMany(u => u.Doacoes)
                .HasForeignKey(d => d.cd_cliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Campanha (para qual campanha)
            builder.HasOne(d => d.Campanha)
                .WithMany(c => c.Doacoes)
                .HasForeignKey(d => d.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
