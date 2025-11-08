using System.ComponentModel.DataAnnotations;

namespace Servidor_PI.Models
{
    public class Noticias
    {
        // ID da noticia (chave primaria)
        [Key]
        public int cd_noticias { get; set; }

        // ID da campanha relacionada
        [Required]
        public int cd_campanha { get; set; }

        // Titulo da noticia
        [Required]
        [MaxLength(200)]
        public string titulo_noticia { get; set; } = string.Empty;

        // Data da noticia
        [Required]
        public DateTime data_noticia { get; set; }

        // Autor (opcional)
        [MaxLength(100)]
        public string? autor { get; set; }

        // Conteudo (opcional)
        public string? conteudo { get; set; }

        // Relacionamento: noticia pertence a uma campanha
        public Campanha Campanha { get; set; } = null!;
    }
}
