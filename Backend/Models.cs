using System.Text.Json.Serialization;

namespace DesafioExpresso;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusPedido
{
    Recebido,
    Preparando,
    Pronto,
    Entregue,
    Cancelado
}

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Tamanho { get; set; } = "M";
    public decimal Preco { get; set; }
    public bool Disponivel { get; set; } = true;
}

public class Pedido
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.Now;
    public StatusPedido Situacao { get; set; } = StatusPedido.Recebido;
    public decimal Total { get; set; }
    public string? Cep { get; set; }
    public string? EnderecoEntrega { get; set; }
    public List<ItemPedido> Itens { get; set; } = new();
}

public class ItemPedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    [JsonIgnore] public Pedido? Pedido { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string? Observacao { get; set; }
}

public record NovoPedidoDto(string Cliente, string? Cep, List<NovoItemDto> Itens);
public record NovoItemDto(int ProdutoId, int Quantidade, string? Observacao);