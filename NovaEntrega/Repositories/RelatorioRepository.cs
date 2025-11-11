using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Servidor_PI.Data;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Repositories
{
    public class RelatorioRepository : IRelatorioRepository
    {
        // Repository de relatórios: CRUD básico e relacionamento com campanha
        private readonly AppDbContext _context;
        private readonly ILogger<RelatorioRepository> _logger;

        // Injeção de dependência: contexto do banco e logger
        public RelatorioRepository(AppDbContext context, ILogger<RelatorioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lista todos os relatórios com campanha relacionada
        public async Task<IEnumerable<Relatorio>> GetAllAsync()
        {
            try
            {
                return await _context.Relatorios
                    .Include(r => r.Campanha)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os relatórios");
                throw;
            }
        }

        // Busca relatório por ID com campanha relacionada
        public async Task<Relatorio?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Relatorios
                    .Include(r => r.Campanha)
                    .FirstOrDefaultAsync(r => r.cd_relatorio == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relatório {Id}", id);
                throw;
            }
        }

        //  Cria novo relatório 
        public async Task<Relatorio> CreateAsync(Relatorio relatorio)
        {
            try
            {
                //  Verifica se a campanha existe
                var campanha = await _context.Campanhas.FindAsync(relatorio.cd_campanha);
                if (campanha == null)
                {
                    throw new InvalidOperationException($"Campanha {relatorio.cd_campanha} não encontrada");
                }

                //  Garante que o EF use a instância já existente da campanha
                relatorio.Campanha = campanha;

                _context.Relatorios.Add(relatorio);
                await _context.SaveChangesAsync();

                return relatorio;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao criar relatório");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar relatório");
                throw;
            }
        }

        // Atualiza relatório existente
        public async Task<Relatorio?> UpdateAsync(int id, Relatorio relatorio)
        {
            try
            {
                var existingRelatorio = await GetByIdAsync(id);
                if (existingRelatorio == null)
                {
                    return null;
                }

                // Validar se campanha existe (se mudou)
                if (existingRelatorio.cd_campanha != relatorio.cd_campanha)
                {
                    var campanha = await _context.Campanhas.FindAsync(relatorio.cd_campanha);
                    if (campanha == null)
                    {
                        throw new InvalidOperationException($"Campanha {relatorio.cd_campanha} não encontrada");
                    }

                    existingRelatorio.Campanha = campanha;
                }

                existingRelatorio.cd_campanha = relatorio.cd_campanha;
                existingRelatorio.tipo_relatorio = relatorio.tipo_relatorio;
                existingRelatorio.valor_gasto = relatorio.valor_gasto;
                existingRelatorio.data_relatorio = relatorio.data_relatorio;

                await _context.SaveChangesAsync();
                return existingRelatorio;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao atualizar relatório {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar relatório {Id}", id);
                throw;
            }
        }

        // Remove relatório por ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var relatorio = await GetByIdAsync(id);
                if (relatorio == null)
                {
                    return false;
                }

                _context.Relatorios.Remove(relatorio);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao deletar relatório {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar relatório {Id}", id);
                throw;
            }
        }
    }
}
