namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;
/// Элемент заказа (фильм в заказе)
public class OrderItemEntity
{
    
    public long Id { get; set; }
    public long OrderId { get; set; }
    public long MovieId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public OrderEntity? Order { get; set; }
}