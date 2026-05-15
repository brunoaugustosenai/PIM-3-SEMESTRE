namespace Projeto.Models;

public record CadastroRequest(string Nome, string Email, string? Telefone, string Senha);
public record LoginRequest(string Email, string Senha);
public record AuthResponse(int Id, string Nome, string Email);
