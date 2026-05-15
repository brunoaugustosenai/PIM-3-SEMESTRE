using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Entities;

[Table("tbitemvenda")]
public class ItemVenda
{
    [Key]
    [Column("id_item_venda")]
    public int Id { get; set; }

    [Column("id_venda")]
    public int VendaId { get; set; }

    [Column("id_produto")]
    public int ProdutoId { get; set; }

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("desconto_percentual")]
    public decimal DescontoPercentual { get; set; }

    public Venda? Venda { get; set; }
    public Produto? Produto { get; set; }
}
