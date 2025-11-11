public class Doacao
{
    [Key]
    public int cd_doacao { get; set; }

    [Required]
    public int cd_cliente { get; set; }

    [Required] 
    public int cd_campanha { get; set; }

    [Required]
    [MaxLength(200)]
    public string nome_doacao { get; set; } = string.Empty;

    [Required]
    public TipoDoacao tipo_doacao { get; set; }

    public FormaArrecadacao? forma_arrecadacao { get; set; }

    public StatusArrecadacao status_arrecadacao { get; set; } = StatusArrecadacao.Pendente;

    // TORNE OPCIONAIS TEMPORARIAMENTE
    public Usuario? Usuario { get; set; }
    public Campanha? Campanha { get; set; }
}
