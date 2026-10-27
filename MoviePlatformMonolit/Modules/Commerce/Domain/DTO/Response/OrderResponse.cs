namespace MoviePlatformMonolit.Modules.Commerce.Domain.DTO.Response;
public class OrderResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}