using DesafioExpresso;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=cafeteria.db"));
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.MapGet("/api/produtos", async (AppDbContext db) => await db.Produtos.ToListAsync());

app.MapGet("/api/pedidos", async (AppDbContext db) =>
    await db.Pedidos.Include(p => p.Itens).OrderByDescending(p => p.DataHora).ToListAsync());

app.MapPost("/api/pedidos", async (NovoPedidoDto dto, AppDbContext db, IHttpClientFactory httpFactory) =>
{
    if (string.IsNullOrWhiteSpace(dto.Cliente))
        return Results.BadRequest("Informe o nome do cliente.");

    if (dto.Itens == null || !dto.Itens.Any())
        return Results.BadRequest("O pedido precisa ter pelo menos um item.");

    var pedido = new Pedido
    {
        Cliente = dto.Cliente.Trim(),
        Cep = dto.Cep
    };

    if (!string.IsNullOrWhiteSpace(dto.Cep))
    {
        try
        {
            var cepLimpo = new string(dto.Cep.Where(char.IsDigit).ToArray());
            var client = httpFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(3);
            
            var viaCep = await client.GetFromJsonAsync<ViaCepResponse>($"https://viacep.com.br/ws/{cepLimpo}/json/");
            if (viaCep != null && string.IsNullOrEmpty(viaCep.Erro))
                pedido.EnderecoEntrega = $"{viaCep.Logradouro}, {viaCep.Bairro} - {viaCep.Localidade}/{viaCep.Uf}";
            else
                pedido.EnderecoEntrega = "CEP não localizado";
        }
        catch
        {
            pedido.EnderecoEntrega = "Serviço de CEP indisponível no momento";
        }
    }

    decimal totalCalculado = 0;

    foreach (var itemDto in dto.Itens)
    {
        if (itemDto.Quantidade <= 0)
            return Results.BadRequest("A quantidade deve ser maior que zero.");

        var produto = await db.Produtos.FindAsync(itemDto.ProdutoId);
        if (produto is null)
            return Results.BadRequest($"Produto ID {itemDto.ProdutoId} não encontrado.");

        if (!produto.Disponivel)
            return Results.BadRequest($"O produto '{produto.Nome}' está indisponível no momento.");

        var item = new ItemPedido
        {
            ProdutoId = produto.Id,
            NomeProduto = produto.Nome,
            Quantidade = itemDto.Quantidade,
            PrecoUnitario = produto.Preco,
            Observacao = itemDto.Observacao
        };

        totalCalculado += item.PrecoUnitario * item.Quantidade;
        pedido.Itens.Add(item);
    }

    pedido.Total = totalCalculado;
    db.Pedidos.Add(pedido);
    await db.SaveChangesAsync();

    return Results.Created($"/api/pedidos/{pedido.Id}", pedido);
});

app.MapPatch("/api/pedidos/{id}/avancar", async (int id, AppDbContext db) =>
{
    var pedido = await db.Pedidos.FindAsync(id);
    if (pedido is null) return Results.NotFound("Pedido não encontrado.");

    if (pedido.Situacao == StatusPedido.Cancelado || pedido.Situacao == StatusPedido.Entregue)
        return Results.BadRequest($"Não é possível avançar um pedido com status '{pedido.Situacao}'.");

    pedido.Situacao = pedido.Situacao switch
    {
        StatusPedido.Recebido => StatusPedido.Preparando,
        StatusPedido.Preparando => StatusPedido.Pronto,
        StatusPedido.Pronto => StatusPedido.Entregue,
        _ => pedido.Situacao
    };

    await db.SaveChangesAsync();
    return Results.Ok(pedido);
});

app.MapPatch("/api/pedidos/{id}/cancelar", async (int id, AppDbContext db) =>
{
    var pedido = await db.Pedidos.FindAsync(id);
    if (pedido is null) return Results.NotFound("Pedido não encontrado.");

    if (pedido.Situacao != StatusPedido.Recebido)
        return Results.BadRequest("O cancelamento só é permitido enquanto o pedido estiver 'Recebido'.");

    pedido.Situacao = StatusPedido.Cancelado;
    await db.SaveChangesAsync();
    return Results.Ok(pedido);
});

app.Run();

record ViaCepResponse(string? Logradouro, string? Bairro, string? Localidade, string? Uf, string? Erro);