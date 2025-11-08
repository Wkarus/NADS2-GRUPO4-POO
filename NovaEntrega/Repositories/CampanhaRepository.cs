using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Servidor_PI.Data;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Repositories
{
    public class CampanhaRepository : ICampanhaRepository
    {
        // Repository de campanha: CRUD basico usando EF Core
        private readonly AppDbContext _context;
        private readonly ILogger<CampanhaRepository> _logger;

        public CampanhaRepository(AppDbContext context, ILogger<CampanhaRepository> logger)
        {
            // Injecao de dependencia: contexto do banco e logger
            _context = context;
            _logger = logger;
        }

        // Lista todas as campanhas
        public async Task<IEnumerable<Campanha>> GetAllAsync()
        {
            try
            {
                return await _context.Campanhas.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as campanhas");
                throw;
            }
        }

        // Busca campanha por ID
        public async Task<Campanha?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Campanhas.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar campanha {Id}", id);
                throw;
            }
        }

        // Cria nova campanha
        public async Task<Campanha> CreateAsync(Campanha campanha)
        {
            try
            {
                // Adiciona e salva no banco
                _context.Campanhas.Add(campanha);
                await _context.SaveChangesAsync();
                return campanha;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao criar campanha");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar campanha");
                throw;
            }
        }

        // Atualiza campanha existente
        public async Task<Campanha?> UpdateAsync(int id, Campanha campanha)
        {
            try
            {
                var existingCampanha = await GetByIdAsync(id);
                if (existingCampanha == null)
                {
                    return null;
                }

                existingCampanha.nome_campanha = campanha.nome_campanha;
                existingCampanha.meta_arrecadacao = campanha.meta_arrecadacao;
                existingCampanha.inicio = campanha.inicio;
                existingCampanha.fim = campanha.fim;

                // Salva alteracoes
                await _context.SaveChangesAsync();
                return existingCampanha;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao atualizar campanha {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar campanha {Id}", id);
                throw;
            }
        }

        // Remove campanha por ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var campanha = await GetByIdAsync(id);
                if (campanha == null)
                {
                    return false;
                }

                // Remove e salva
                _context.Campanhas.Remove(campanha);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao deletar campanha {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar campanha {Id}", id);
                throw;
            }
        }
    }
}
