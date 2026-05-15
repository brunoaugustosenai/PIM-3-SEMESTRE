using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Entities;

[Table("tbvenda")]
public class Venda
{
    [Key]
    [Column("id_venda")]
    public int Id { get; set; }

    [Column("id_cliente")]
    public int ClienteId { get; set; }

    [Column("data_venda")]
    public DateTime DataVenda { get; set; } = DateTime.Now;

    [Column("status_venda")]
    public string StatusVenda { get; set; } = "APROVADO";

    [Column("status_pagamento")]
    public string StatusPagamento { get; set; } = "PENDENTE";

    [Column("valor_total")]
    public decimal ValorTotal { get; set; }

    public List<ItemVenda> Itens { get; set; } = new();
}
