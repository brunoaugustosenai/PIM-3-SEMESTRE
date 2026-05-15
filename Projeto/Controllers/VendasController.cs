using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Entities;
using Projeto.Models;

namespace Projeto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VendasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var vendas = await _context.Vendas
            .AsNoTracking()
            .Include(v => v.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(v => v.DataVenda)
            .Select(v => new
            {
                v.Id,
                v.ClienteId,
                v.DataVenda,
                v.StatusVenda,
                v.StatusPagamento,
                v.ValorTotal,
                Itens = v.Itens.Select(i => new
                {
                    i.ProdutoId,
                    Produto = i.Produto != null ? i.Produto.Nome : string.Empty,
                    i.Quantidade,
                    i.PrecoUnitario
                })
            })
            .ToListAsync();

        return Ok(vendas);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarVendaRequest request)
    {
        if (request.ClienteId <= 0)
            return BadRequest(new { mensagem = "Cliente não informado." });

        if (request.Itens.Count == 0)
            return BadRequest(new { mensagem = "Carrinho vazio." });

        var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == request.ClienteId && c.Ativo);
        if (!clienteExiste)
            return BadRequest(new { mensagem = "Cliente inválido. Faça login novamente." });

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var venda = new Venda
            {
                ClienteId = request.ClienteId,
                DataVenda = DateTime.Now,
                StatusVenda = "APROVADO",
                StatusPagamento = "PENDENTE",
                ValorTotal = 0
            };

            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            decimal total = 0;

            foreach (var itemRequest in request.Itens)
            {
                if (itemRequest.Quantidade <= 0)
                    return BadRequest(new { mensagem = "Quantidade inválida." });

                var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == itemRequest.ProdutoId && p.Ativo);
                if (produto is null)
                    return BadRequest(new { mensagem = $"Produto {itemRequest.ProdutoId} não encontrado." });

                if (produto.Estoque < itemRequest.Quantidade)
                    return BadRequest(new { mensagem = $"Estoque insuficiente para {produto.Nome}. Disponível: {produto.Estoque}." });

                produto.Estoque -= itemRequest.Quantidade;

                var item = new ItemVenda
                {
                    VendaId = venda.Id,
                    ProdutoId = produto.Id,
                    Quantidade = itemRequest.Quantidade,
                    PrecoUnitario = produto.PrecoVenda,
                    DescontoPercentual = 0
                };

                total += item.PrecoUnitario * item.Quantidade;
                _context.ItensVenda.Add(item);
            }

            venda.ValorTotal = total;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { venda.Id, venda.ValorTotal, mensagem = "Compra finalizada com sucesso!" });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
