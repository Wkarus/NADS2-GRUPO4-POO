using Microsoft.EntityFrameworkCore;
using Servidor_PI.Models;

namespace Servidor_PI.Data
{
    // DbContext: classe que representa o banco de dados na aplicacao
    // Aqui declaramos as tabelas (DbSet) e aplicamos os mapeamentos
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tabelas da aplicacao
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Campanha> Campanhas { get; set; }
        public DbSet<Doacao> Doacoes { get; set; }
        public DbSet<Noticias> Noticias { get; set; }
        public DbSet<Relatorio> Relatorios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aplica mapeamentos das entidades (pasta Data/Maps)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
