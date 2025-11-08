using Servidor_PI.Models;

namespace Servidor_PI.Repositories.Interfaces
{
    public interface IRelatorioRepository
    {
        // Lista todos os relatorios.
        Task<IEnumerable<Relatorio>> GetAllAsync();

        //Busca um relatorio pelo ID.
        Task<Relatorio?> GetByIdAsync(int id);

        // Cria um novo relatorio.
        Task<Relatorio> CreateAsync(Relatorio relatorio);

        // Atualiza um relatorio existente.
        Task<Relatorio?> UpdateAsync(int id, Relatorio relatorio);


        // Remove um relatorio pelo ID
        Task<bool> DeleteAsync(int id);
    }
}
