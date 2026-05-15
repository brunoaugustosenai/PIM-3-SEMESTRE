using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto.Data;

namespace Projeto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Ativo)
            .OrderBy(c => c.Nome)
            .Select(c => new { c.Id, c.Nome, c.Email, c.Telefone, c.DataCadastro })
            .ToListAsync();

        return Ok(clientes);
    }
}
