using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Servidor_PI.Models
{
    public class Relatorio
    {
        [Key]
        public int cd_relatorio { get; set; }

        // 🔹 Este campo garante que toda notícia tenha uma campanha
        [Required]
        [ForeignKey("Campanha")]
        public int cd_campanha { get; set; }

        [Required]
        [MaxLength(100)]
        public string tipo_relatorio { get; set; } = string.Empty;

        [Required]
        public decimal valor_gasto { get; set; }

        [Required]
        public DateTime data_relatorio { get; set; }

        //  
        // O EF entende o relacionamento automaticamente pelo cd_campanha
        [ValidateNever]
        public Campanha? Campanha { get; set;};
    }
}
