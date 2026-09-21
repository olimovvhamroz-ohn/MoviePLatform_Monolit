namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.REQUEST;

public class CreateOrderRequest
{
    public int UserId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
    public string? Notes { get; set; }
}

public class OrderItemRequest
{
    public int MovieId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }
}

public class UpdateOrderRequest
{
    public int Id { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

public class CancelOrderRequest
{
    public int OrderId { get; set; }
    public string? Reason { get; set; }
}

public class RefundOrderRequest
{
    public int OrderId { get; set; }
    public string? Reason { get; set; }
}