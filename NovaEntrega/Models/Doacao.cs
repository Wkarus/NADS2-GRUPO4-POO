using System.ComponentModel.DataAnnotations;
using Servidor_PI.Enums;

namespace Servidor_PI.Models
{
    // Entidade doacao: guarda quem doou, para qual campanha e o tipo
    public class Doacao
    {
        // ID da doacao (chave primaria)
        [Key]
        public int cd_doacao { get; set; }              // ID da doacao

        // IDs de relacionamento
        [Required]
        public int cd_cliente { get; set; }              // ID do usuario (quem doa)

        [Required]
        public int cd_campanha { get; set; }             // ID da campanha

        // Informacoes principais da doacao
        [Required]
        [MaxLength(200)]
        public string nome_doacao { get; set; } = string.Empty; // Nome/descricao da doacao

        // Tipo/forma/status da arrecadacao
        [Required]
        public TipoDoacao tipo_doacao { get; set; }      // Tipo (ex.: Dinheiro, Alimento)

        public FormaArrecadacao? forma_arrecadacao { get; set; } // Forma de arrecadacao

        public StatusArrecadacao status_arrecadacao { get; set; } = StatusArrecadacao.Pendente; // Status

        // Relacionamentos (ligacao com outras tabelas)
        public Usuario Usuario { get; set; } = null!;    // Usuario dono da doacao
        public Campanha Campanha { get; set; } = null!;  // Campanha relacionada
    }
}
