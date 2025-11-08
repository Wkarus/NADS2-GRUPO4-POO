using Servidor_PI.Models;

namespace Servidor_PI.Repositories.Interfaces
{
    public interface INoticiasRepository
    {
        // Lista todas as noticias.
        Task<IEnumerable<Noticias>> GetAllAsync();

        // Busca uma noticia pelo ID.
        Task<Noticias?> GetByIdAsync(int id);

      
        //Cria uma nova noticia.
    
        Task<Noticias> CreateAsync(Noticias noticias);

        
        // Atualiza uma noticia existente.
        
        Task<Noticias?> UpdateAsync(int id, Noticias noticias);

        
        // Remove uma noticia pelo ID.
        
        Task<bool> DeleteAsync(int id);
    }
}
