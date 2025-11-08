using System.ComponentModel.DataAnnotations;

namespace Servidor_PI.Models
{
    public class Campanha
    {
        // ID da campanha (chave primaria)
        [Key]
        public int cd_campanha { get; set; }

        // Nome da campanha
        [Required]
        [MaxLength(200)]
        public string nome_campanha { get; set; } = string.Empty;

        // Meta financeira da campanha (opcional)
        public decimal? meta_arrecadacao { get; set; }

        // Datas de inicio e fim (opcionais)
        public DateTime? inicio { get; set; }

        public DateTime? fim { get; set; }

        // Relacionamentos
        public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();
        public ICollection<Noticias> Noticias { get; set; } = new List<Noticias>();
        public ICollection<Relatorio> Relatorios { get; set; } = new List<Relatorio>();
    }
}
