

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.Response;

public class PurchaseResponse
{
    
    public long UserId { get; set; }
    public long MovieId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public DateTime? RentalStartDeadline { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
}