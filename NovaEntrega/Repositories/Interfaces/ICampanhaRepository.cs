using Servidor_PI.Models;

namespace Servidor_PI.Repositories.Interfaces
{
    public interface ICampanhaRepository
    {
      
        // Lista todas as campanhas.
        Task<IEnumerable<Campanha>> GetAllAsync();

        
        // Busca uma campanha pelo ID.
        Task<Campanha?> GetByIdAsync(int id);

       
        // Cria uma nova campanha.
        Task<Campanha> CreateAsync(Campanha campanha);

       
        // Atualiza uma campanha existente.
        Task<Campanha?> UpdateAsync(int id, Campanha campanha);

     
        // Remove uma campanha pelo ID.
        Task<bool> DeleteAsync(int id);
    }
}

