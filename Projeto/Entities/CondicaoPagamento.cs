using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto.Entities;

[Table("tbcondicaopagamento")]
public class CondicaoPagamento
{
    [Key]
    [Column("id_condicao_pagamento")]
    public int Id { get; set; }

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("numero_parcelas")]
    public int NumeroParcelas { get; set; }

    [Column("taxa_juros_percentual")]
    public decimal TaxaJurosPercentual { get; set; }

    [Column("dias_primeira_parcela")]
    public int DiasPrimeiraParcela { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;
}
