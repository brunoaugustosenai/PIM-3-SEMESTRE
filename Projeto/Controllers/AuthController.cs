using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;
using Projeto.Entities;
using Projeto.Models;

namespace Projeto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("cadastro")]
    public async Task<ActionResult<AuthResponse>> Cadastrar(CadastroRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            return BadRequest(new { mensagem = "Nome, e-mail e senha são obrigatórios." });

        var email = request.Email.Trim().ToLower();
        var existe = await _context.Clientes.AnyAsync(c => c.Email == email);
        if (existe)
            return Conflict(new { mensagem = "E-mail já cadastrado." });

        var cliente = new Cliente
        {
            Nome = request.Nome.Trim(),
            Email = email,
            Telefone = request.Telefone,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            DataCadastro = DateTime.Now,
            Ativo = true
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(cliente.Id, cliente.Nome, cliente.Email));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLower();
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == email && c.Ativo);
        if (cliente is null)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        var senhaOk = false;
        try
        {
            senhaOk = BCrypt.Net.BCrypt.Verify(request.Senha, cliente.SenhaHash);
        }
        catch
        {
            senhaOk = request.Senha == cliente.SenhaHash;
        }

        if (!senhaOk)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        return Ok(new AuthResponse(cliente.Id, cliente.Nome, cliente.Email));
    }
}
