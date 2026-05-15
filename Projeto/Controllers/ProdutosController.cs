using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Entities;
using Projeto.Models;

namespace Projeto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Listar([FromQuery] string? busca)
    {
        var query = _context.Produtos.AsNoTracking().Where(p => p.Ativo);
        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(p => p.Nome.Contains(busca) || (p.Descricao != null && p.Descricao.Contains(busca)));

        return Ok(await query.OrderBy(p => p.Nome).ToListAsync());
    }

    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<Produto>>> ListarAdmin()
    {
        return Ok(await _context.Produtos.AsNoTracking().OrderBy(p => p.Nome).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> Criar(ProdutoRequest request)
    {
        var produto = new Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            PrecoVenda = request.PrecoVenda,
            Estoque = request.Estoque,
            ImagemUrl = request.ImagemUrl,
            Ativo = request.Ativo
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Obter), new { id = produto.Id }, produto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> Obter(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        return produto is null ? NotFound(new { mensagem = "Produto não encontrado." }) : Ok(produto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ProdutoRequest request)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        produto.Nome = request.Nome;
        produto.Descricao = request.Descricao;
        produto.PrecoVenda = request.PrecoVenda;
        produto.Estoque = request.Estoque;
        produto.ImagemUrl = request.ImagemUrl;
        produto.Ativo = request.Ativo;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        produto.Ativo = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
