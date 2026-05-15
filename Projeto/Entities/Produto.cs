using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Entities;

[Table("tbproduto")]
public class Produto
{
    [Key]
    [Column("id_produto")]
    public int Id { get; set; }

    [Required, MaxLength(150)]
    [Column("nome_produto")]
    public string Nome { get; set; } = string.Empty;

    [Column("descricao")]
    public string? Descricao { get; set; }

    [Column("preco_venda")]
    public decimal PrecoVenda { get; set; }

    [Column("estoque")]
    public int Estoque { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("imagem_url")]
    public string? ImagemUrl { get; set; }
}
