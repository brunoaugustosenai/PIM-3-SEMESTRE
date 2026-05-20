namespace Projeto.Models;

public class CriarVendaRequest
{
    public int ClienteId { get; set; }

    public string? FormaPagamento { get; set; }

    public string? CartaoNumero { get; set; }

    public string? CartaoValidade { get; set; }

    public string? CartaoCvv { get; set; }

    public List<ItemVendaRequest> Itens { get; set; } = new();
}

public class ItemVendaRequest
{
    public int ProdutoId { get; set; }

    public int Quantidade { get; set; }
}