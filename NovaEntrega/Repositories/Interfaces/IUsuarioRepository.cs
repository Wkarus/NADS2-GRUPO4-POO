using Servidor_PI.Models;

namespace Servidor_PI.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        // Lista todos os usuarios.
        Task<IEnumerable<Usuario>> GetAllAsync();

        // Busca um usuario pelo ID.
        Task<Usuario?> GetByIdAsync(int id);

       
        //Busca um usuario pelo email.
        Task<Usuario?> GetByEmailAsync(string email);

        // Busca um usuario pelo nome de usuario (login).
        Task<Usuario?> GetByNomeUsuarioAsync(string nomeUsuario);

        // Cria um novo usuario.
        Task<Usuario> CreateAsync(Usuario usuario);


        /// Atualiza um usuario existente.
        Task<Usuario?> UpdateAsync(int id, Usuario usuario);

        /// Remove um usuario pelo ID.
        Task<bool> DeleteAsync(int id);
    }
}

