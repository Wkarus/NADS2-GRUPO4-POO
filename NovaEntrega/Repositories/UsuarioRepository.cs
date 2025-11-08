using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Servidor_PI.Data;
using Servidor_PI.Models;
using Servidor_PI.Repositories.Interfaces;

namespace Servidor_PI.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        // Repository de usuarios: CRUD basico e relacionamento com doacoes
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        // Injecao de dependencia: contexto do banco e logger
        public UsuarioRepository(AppDbContext context, ILogger<UsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lista todos os usuarios
        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                return await _context.Usuarios.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
                throw;
            }
        }

        // Busca usuario por ID
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Usuarios.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário {Id}", id);
                throw;
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Usuarios.FirstOrDefaultAsync(u => u.email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email {Email}", email);
                throw;
            }
        }

        public async Task<Usuario?> GetByNomeUsuarioAsync(string nomeUsuario)
        {
            try
            {
                return await _context.Usuarios.FirstOrDefaultAsync(u => u.nome_usuario == nomeUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por nome {NomeUsuario}", nomeUsuario);
                throw;
            }
        }

        // Cria novo usuario
        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            try
            {
                // Validar se email ja existe
                if (await GetByEmailAsync(usuario.email) != null)
                {
                    throw new InvalidOperationException($"Email {usuario.email} já está em uso");
                }

                // Validar se nome_usuario ja existe
                if (await GetByNomeUsuarioAsync(usuario.nome_usuario) != null)
                {
                    throw new InvalidOperationException($"Nome de usuário {usuario.nome_usuario} já está em uso");
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao criar usuario");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuario");
                throw;
            }
        }

        // Atualiza usuario existente
        public async Task<Usuario?> UpdateAsync(int id, Usuario usuario)
        {
            try
            {
                var existingUsuario = await GetByIdAsync(id);
                if (existingUsuario == null)
                {
                    return null;
                }

                // Validar email unico (se mudou)
                if (existingUsuario.email != usuario.email && await GetByEmailAsync(usuario.email) != null)
                {
                    throw new InvalidOperationException($"Email {usuario.email} já está em uso");
                }

                // Validar nome_usuario unico (se mudou)
                if (existingUsuario.nome_usuario != usuario.nome_usuario && await GetByNomeUsuarioAsync(usuario.nome_usuario) != null)
                {
                    throw new InvalidOperationException($"Nome de usuário {usuario.nome_usuario} já está em uso");
                }

                existingUsuario.nome_completo = usuario.nome_completo;
                existingUsuario.telefone = usuario.telefone;
                existingUsuario.cpf = usuario.cpf;
                existingUsuario.cep = usuario.cep;
                existingUsuario.nome_usuario = usuario.nome_usuario;
                existingUsuario.senha = usuario.senha;
                existingUsuario.email = usuario.email;

                await _context.SaveChangesAsync();
                return existingUsuario;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao atualizar usuario {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuario {Id}", id);
                throw;
            }
        }

        // Remove usuario por ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await GetByIdAsync(id);
                if (usuario == null)
                {
                    return false;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException sqlEx)
            {
                _logger.LogError(sqlEx, "Erro de SQL ao deletar usuario {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuario {Id}", id);
                throw;
            }
        }
    }
}
