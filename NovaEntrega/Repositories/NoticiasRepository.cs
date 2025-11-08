using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Servidor_PI.Data;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Repositories
{
    public class NoticiasRepository : INoticiasRepository
    {
        // Repository de noticias: CRUD basico e relacionamento com campanha
        private readonly AppDbContext _context;
        private readonly ILogger<NoticiasRepository> _logger;

        // Injecao de dependencia: contexto do banco e logger
        public NoticiasRepository(AppDbContext context, ILogger<NoticiasRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lista todas as noticias com campanha relacionada
        public async Task<IEnumerable<Noticias>> GetAllAsync()
        {
            try
            {
                return await _context.Noticias
                    .Include(n => n.Campanha)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as notícias");
                throw;
            }
        }

        // Busca noticia por ID com campanha relacionada
        public async Task<Noticias?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Noticias
                    .Include(n => n.Campanha)
                    .FirstOrDefaultAsync(n => n.cd_noticias == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar notícia {Id}", id);
                throw;
            }
        }

        // Cria nova noticia
        public async Task<Noticias> CreateAsync(Noticias noticias)
        {
            try
            {
                // Validar se campanha existe
                var campanha = await _context.Campanhas.FindAsync(noticias.cd_campanha);
                if (campanha == null)
                {
                    throw new InvalidOperationException($"Campanha {noticias.cd_campanha} não encontrada");
                }

                _context.Noticias.Add(noticias);
                await _context.SaveChangesAsync();
                return noticias;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao criar notícia");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar notícia");
                throw;
            }
        }

        // Atualiza noticia existente
        public async Task<Noticias?> UpdateAsync(int id, Noticias noticias)
        {
            try
            {
                var existingNoticia = await GetByIdAsync(id);
                if (existingNoticia == null)
                {
                    return null;
                }

                // Validar se campanha existe (se mudou)
                if (existingNoticia.cd_campanha != noticias.cd_campanha)
                {
                    var campanha = await _context.Campanhas.FindAsync(noticias.cd_campanha);
                    if (campanha == null)
                    {
                        throw new InvalidOperationException($"Campanha {noticias.cd_campanha} não encontrada");
                    }
                }

                existingNoticia.cd_campanha = noticias.cd_campanha;
                existingNoticia.titulo_noticia = noticias.titulo_noticia;
                existingNoticia.data_noticia = noticias.data_noticia;
                existingNoticia.autor = noticias.autor;
                existingNoticia.conteudo = noticias.conteudo;

                await _context.SaveChangesAsync();
                return existingNoticia;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao atualizar notícia {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar notícia {Id}", id);
                throw;
            }
        }

        // Remove noticia por ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var noticia = await GetByIdAsync(id);
                if (noticia == null)
                {
                    return false;
                }

                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao deletar notícia {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar notícia {Id}", id);
                throw;
            }
        }
    }
}
