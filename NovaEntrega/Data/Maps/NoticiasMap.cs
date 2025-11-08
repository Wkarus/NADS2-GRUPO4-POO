using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servidor_PI.Models;

namespace Servidor_PI.Data.Maps
{
    public class NoticiasMap : IEntityTypeConfiguration<Noticias>
    {
        public void Configure(EntityTypeBuilder<Noticias> builder)
        {
            // Tabela noticias
            builder.ToTable("Noticias");

            // Chave primaria
            builder.HasKey(n => n.cd_noticias);

            builder.Property(n => n.cd_noticias)
                .ValueGeneratedOnAdd();

            // Relacionamento com campanha
            builder.Property(n => n.cd_campanha)
                .IsRequired();

            // Campos principais
            builder.Property(n => n.titulo_noticia)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.data_noticia)
                .IsRequired()
                .HasColumnType("DATE");

            builder.Property(n => n.autor)
                .HasMaxLength(100);

            builder.Property(n => n.conteudo)
                .HasColumnType("TEXT");

            // Indice por data para ordenar/buscar
            builder.HasIndex(n => n.data_noticia)
                .HasDatabaseName("idx_noticias_data");

            // Relacionamento: noticia pertence a uma campanha
            builder.HasOne(n => n.Campanha)
                .WithMany(c => c.Noticias)
                .HasForeignKey(n => n.cd_campanha)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
