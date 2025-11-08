using System.ComponentModel.DataAnnotations;
using Servidor_PI.Enums;

namespace Servidor_PI.Models
{
    public class Relatorio
    {
        // ID do relatorio (chave primaria)
        [Key]
        public int cd_relatorio { get; set; }

        // ID da campanha relacionada
        [Required]
        public int cd_campanha { get; set; }

        // Tipo de relatorio (enum)
        [Required]
        public TipoRelatorio tipo_relatorio { get; set; }

        // Valor gasto (opcional)
        public decimal? valor_gasto { get; set; }

        // Data do relatorio
        [Required]
        public DateTime data_relatorio { get; set; }

        // Relacionamento: relatorio pertence a uma campanha
        public Campanha Campanha { get; set; } = null!;
    }
}
