namespace Projeto.Models;

public record ItemVendaRequest(int ProdutoId, int Quantidade);
public record CriarVendaRequest(int ClienteId, List<ItemVendaRequest> Itens);
