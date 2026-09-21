using MoviePLatform_Monolit.Modules.Commerce.Domain.Enum;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

public class PaymentEntity
{
    //platyoch
    public long Id { get; set; }
    public long OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public OrderEntity? Order { get; set; }
}