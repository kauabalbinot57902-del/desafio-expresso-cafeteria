using Microsoft.EntityFrameworkCore;

namespace DesafioExpresso;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>().HasData(
            new Produto { Id = 1, Nome = "Café Expresso", Categoria = "Café", Tamanho = "P", Preco = 6.50m, Disponivel = true },
            new Produto { Id = 2, Nome = "Cappuccino Cremoso", Categoria = "Café", Tamanho = "M", Preco = 11.00m, Disponivel = true },
            new Produto { Id = 3, Nome = "Chá Gelado de Pêssego", Categoria = "Chá", Tamanho = "G", Preco = 9.50m, Disponivel = true },
            new Produto { Id = 4, Nome = "Pão de Queijo Mineiro", Categoria = "Salgado", Tamanho = "M", Preco = 7.00m, Disponivel = true },
            new Produto { Id = 5, Nome = "Croissant de Chocolate", Categoria = "Salgado", Tamanho = "G", Preco = 12.50m, Disponivel = false }
        );
    }
}