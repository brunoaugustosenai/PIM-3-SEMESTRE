using Microsoft.EntityFrameworkCore;
using Projeto.Entities;

namespace Projeto.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<CondicaoPagamento> CondicoesPagamento => Set<CondicaoPagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Produto>()
            .Property(p => p.PrecoVenda)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Venda>()
            .Property(v => v.ValorTotal)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ItemVenda>()
            .ToTable("tbitemvenda", tb => tb.HasTrigger("trg_verificar_estoque_venda"));

        modelBuilder.Entity<ItemVenda>()
            .Property(i => i.PrecoUnitario)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ItemVenda>()
            .Property(i => i.DescontoPercentual)
            .HasPrecision(5, 2);

        modelBuilder.Entity<ItemVenda>()
            .HasOne(i => i.Venda)
            .WithMany(v => v.Itens)
            .HasForeignKey(i => i.VendaId);

        modelBuilder.Entity<ItemVenda>()
            .HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId);

        modelBuilder.Entity<CondicaoPagamento>()
            .Property(c => c.TaxaJurosPercentual)
            .HasPrecision(5, 2);
    }
}