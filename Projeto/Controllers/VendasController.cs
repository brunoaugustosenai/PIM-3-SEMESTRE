using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Entities;
using Projeto.Models;
using System.Text.RegularExpressions;

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
            return BadRequest(new { mensagem = "Cliente não informado. Faça login para finalizar a compra." });

        if (request.Itens == null || request.Itens.Count == 0)
            return BadRequest(new { mensagem = "Carrinho vazio." });

        var clienteExiste = await _context.Clientes
            .AnyAsync(c => c.Id == request.ClienteId && c.Ativo);

        if (!clienteExiste)
            return BadRequest(new { mensagem = "Cliente inválido. Faça login novamente." });

        string formaPagamento = NormalizarFormaPagamento(request.FormaPagamento);

        if (string.IsNullOrWhiteSpace(formaPagamento))
            return BadRequest(new { mensagem = "Selecione uma forma de pagamento." });

        if (formaPagamento != "pix" && formaPagamento != "boleto" && formaPagamento != "cartao")
            return BadRequest(new { mensagem = "Forma de pagamento inválida." });

        if (formaPagamento == "cartao")
        {
            if (!CartaoValido(request.CartaoNumero, request.CartaoValidade, request.CartaoCvv))
            {
                return BadRequest(new
                {
                    mensagem = "Dados do cartão inválidos. Verifique número, validade e CVV."
                });
            }
        }

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

                var produto = await _context.Produtos
                    .FirstOrDefaultAsync(p => p.Id == itemRequest.ProdutoId && p.Ativo);

                if (produto is null)
                    return BadRequest(new { mensagem = $"Produto {itemRequest.ProdutoId} não encontrado." });

                if (produto.Estoque < itemRequest.Quantidade)
                {
                    return BadRequest(new
                    {
                        mensagem = $"Estoque insuficiente para {produto.Nome}. Disponível: {produto.Estoque}."
                    });
                }

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

            return Ok(new
            {
                venda.Id,
                venda.ValorTotal,
                FormaPagamento = formaPagamento,
                mensagem = "Compra finalizada com sucesso!"
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                mensagem = "Erro ao finalizar venda.",
                detalhe = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    private static string NormalizarFormaPagamento(string? formaPagamento)
    {
        if (string.IsNullOrWhiteSpace(formaPagamento))
            return "";

        var valor = formaPagamento.Trim().ToLower();

        valor = valor
            .Replace("ã", "a")
            .Replace("á", "a")
            .Replace("à", "a")
            .Replace("â", "a")
            .Replace("é", "e")
            .Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ô", "o")
            .Replace("ú", "u")
            .Replace("ç", "c");

        if (valor.Contains("pix"))
            return "pix";

        if (valor.Contains("boleto"))
            return "boleto";

        if (valor.Contains("cartao") || valor.Contains("credito"))
            return "cartao";

        return valor;
    }

    private static bool CartaoValido(string? numero, string? validade, string? cvv)
    {
        if (string.IsNullOrWhiteSpace(numero) ||
            string.IsNullOrWhiteSpace(validade) ||
            string.IsNullOrWhiteSpace(cvv))
            return false;

        numero = new string(numero.Where(char.IsDigit).ToArray());
        cvv = new string(cvv.Where(char.IsDigit).ToArray());

        if (numero.Length < 13 || numero.Length > 19)
            return false;

        if (cvv.Length < 3 || cvv.Length > 4)
            return false;

        if (!Regex.IsMatch(validade, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            return false;

        var partes = validade.Split('/');
        int mes = int.Parse(partes[0]);
        int ano = 2000 + int.Parse(partes[1]);

        var hoje = DateTime.Now;
        var vencimento = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

        if (vencimento < hoje.Date)
            return false;

        int soma = 0;
        bool dobrar = false;

        for (int i = numero.Length - 1; i >= 0; i--)
        {
            int digito = numero[i] - '0';

            if (dobrar)
            {
                digito *= 2;

                if (digito > 9)
                    digito -= 9;
            }

            soma += digito;
            dobrar = !dobrar;
        }

        return soma % 10 == 0;
    }
}