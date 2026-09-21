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
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderSummaryResponse
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
}