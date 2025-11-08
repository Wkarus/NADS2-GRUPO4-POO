using System.ComponentModel.DataAnnotations;

namespace Servidor_PI.Models
{
    // Entidade usuario: dados de login e informacoes pessoais basicas
    public class Usuario
    {
        // ID do usuario (chave primaria)
        [Key]
        public int cd_cliente { get; set; }                    // ID do usuario

        // Nome completo do usuario
        [Required]
        [MaxLength(200)]
        public string nome_completo { get; set; } = string.Empty; // Nome completo

        // Contatos (opcionais)
        [MaxLength(20)]
        public string? telefone { get; set; }                   // Telefone (opcional)

        [MaxLength(11)]
        public string? cpf { get; set; }                        // CPF (opcional)

        [MaxLength(10)]
        public string? cep { get; set; }                        // CEP (opcional)

        // Credenciais de acesso
        [Required]
        [MaxLength(50)]
        public string nome_usuario { get; set; } = string.Empty; // Usuario de login

        [Required]
        [MaxLength(255)]
        public string senha { get; set; } = string.Empty;        // Senha

        [Required]
        [MaxLength(100)]
    public string email { get; set; } = string.Empty;        // Email

        // Relacionamento: um usuario pode fazer varias doacoes
        public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();
    }
}
