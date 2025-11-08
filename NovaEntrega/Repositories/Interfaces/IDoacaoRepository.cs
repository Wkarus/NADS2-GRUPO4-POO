using Servidor_PI.Models;

namespace Servidor_PI.Repositories.Interfaces
{
    public interface IDoacaoRepository
    {
       
        // Lista todas as doacoes.
       
        Task<IEnumerable<Doacao>> GetAllAsync();

        
        // Busca uma doacao pelo ID.
        
        Task<Doacao?> GetByIdAsync(int id);

        
        // Cria uma nova doacao.
       
        Task<Doacao> CreateAsync(Doacao doacao);

        
        // Atualiza uma doacao existente.
        
        Task<Doacao?> UpdateAsync(int id, Doacao doacao);

        
        // Remove uma doacao pelo ID.
        
        Task<bool> DeleteAsync(int id);
    }
}
