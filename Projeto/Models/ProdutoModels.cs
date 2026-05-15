namespace Projeto.Models;

public record ProdutoRequest(string Nome, string? Descricao, decimal PrecoVenda, int Estoque, string? ImagemUrl, bool Ativo = true);
