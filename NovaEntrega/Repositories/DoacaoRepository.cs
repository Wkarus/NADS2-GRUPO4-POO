using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Servidor_PI.Data;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Repositories
{
    public class DoacaoRepository : IDoacaoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoacaoRepository> _logger;

        public DoacaoRepository(AppDbContext context, ILogger<DoacaoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Doacao>> GetAllAsync()
        {
            try
            {
                return await _context.Doacoes
                    .Include(d => d.Usuario)
                    .Include(d => d.Campanha)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as doações");
                throw;
            }
        }

        public async Task<Doacao?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Doacoes
                    .Include(d => d.Usuario)
                    .Include(d => d.Campanha)
                    .FirstOrDefaultAsync(d => d.cd_doacao == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar doação {Id}", id);
                throw;
            }
        }

        public async Task<Doacao> CreateAsync(Doacao doacao)
        {
            try
            {
                // Validar se usuario existe
                var usuario = await _context.Usuarios.FindAsync(doacao.cd_cliente);
                if (usuario == null)
                {
                    throw new InvalidOperationException($"Usuário {doacao.cd_cliente} não encontrado");
                }

                // Validar se campanha existe
                var campanha = await _context.Campanhas.FindAsync(doacao.cd_campanha);
                if (campanha == null)
                {
                    throw new InvalidOperationException($"Campanha {doacao.cd_campanha} não encontrada");
                }

                _context.Doacoes.Add(doacao);
                await _context.SaveChangesAsync();
                return doacao;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao criar doação");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar doação");
                throw;
            }
        }

        // Atualiza doacao existente
        public async Task<Doacao?> UpdateAsync(int id, Doacao doacao)
        {
            try
            {
                var existingDoacao = await GetByIdAsync(id);
                if (existingDoacao == null)
                {
                    return null;
                }

                // Validar se usuario existe (se mudou)
                if (existingDoacao.cd_cliente != doacao.cd_cliente)
                {
                    var usuario = await _context.Usuarios.FindAsync(doacao.cd_cliente);
                    if (usuario == null)
                    {
                        throw new InvalidOperationException($"Usuário {doacao.cd_cliente} não encontrado");
                    }
                }

                // Validar se campanha existe (se mudou)
                if (existingDoacao.cd_campanha != doacao.cd_campanha)
                {
                    var campanha = await _context.Campanhas.FindAsync(doacao.cd_campanha);
                    if (campanha == null)
                    {
                        throw new InvalidOperationException($"Campanha {doacao.cd_campanha} não encontrada");
                    }
                }

                existingDoacao.cd_cliente = doacao.cd_cliente;
                existingDoacao.cd_campanha = doacao.cd_campanha;
                existingDoacao.nome_doacao = doacao.nome_doacao;
                existingDoacao.tipo_doacao = doacao.tipo_doacao;
                existingDoacao.forma_arrecadacao = doacao.forma_arrecadacao;
                existingDoacao.status_arrecadacao = doacao.status_arrecadacao;

                await _context.SaveChangesAsync();
                return existingDoacao;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao atualizar doação {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar doação {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var doacao = await GetByIdAsync(id);
                if (doacao == null)
                {
                    return false;
                }

                _context.Doacoes.Remove(doacao);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao deletar doação {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar doação {Id}", id);
                throw;
            }
        }
    }
}
