using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servidor_PI.Models;

namespace Servidor_PI.Data.Maps
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            // Tabela usuario
            builder.ToTable("Usuario");

            // Chave primaria
            builder.HasKey(u => u.cd_cliente);

            builder.Property(u => u.cd_cliente)
                .ValueGeneratedOnAdd();

            // Dados pessoais basicos
            builder.Property(u => u.nome_completo)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.telefone)
                .HasMaxLength(20);

            builder.Property(u => u.cpf)
                .HasMaxLength(11);

            builder.Property(u => u.cep)
                .HasMaxLength(10);

            // Credenciais de login
            builder.Property(u => u.nome_usuario)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.senha)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.email)
                .IsRequired()
                .HasMaxLength(100);

            // Campos unicos para evitar duplicidade
            builder.HasIndex(u => u.cpf)
                .IsUnique()
                .HasDatabaseName("idx_usuario_cpf");

            builder.HasIndex(u => u.nome_usuario)
                .IsUnique();

            builder.HasIndex(u => u.email)
                .IsUnique()
                .HasDatabaseName("idx_usuario_email");

            // Relacionamento: um usuario pode ter varias doacoes
            builder.HasMany(u => u.Doacoes)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.cd_cliente)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
