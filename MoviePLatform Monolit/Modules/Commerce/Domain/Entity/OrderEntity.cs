using MoviePLatform_Monolit.Modules.Commerce.Domain.Enum;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;


/// <summary>
/// Заказ (покупка фильмов)
/// </summary>
public class OrderEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItemEntity> Items { get; set; } = new();
    public PaymentEntity? Payment { get; set; }
    public string? Notes { get; set; }
}


