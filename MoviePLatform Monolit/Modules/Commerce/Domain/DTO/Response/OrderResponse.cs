namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.RESPONSE;

public class OrderItemResponse
{
    public long MovieId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class OrderResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}