using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Entities;

[Table("tbcadastrocliente")]
public class Cliente
{
    [Key]
    [Column("id_cliente")]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    [Column("nome_cliente")]
    public string Nome { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [Column("telefone_celular")]
    public string? Telefone { get; set; }

    [Required, MaxLength(255)]
    [Column("senha")]
    public string SenhaHash { get; set; } = string.Empty;

    [Column("data_cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
