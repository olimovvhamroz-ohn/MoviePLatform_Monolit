

namespace MoviePLatform_Monolit.Users.DTO.RESPONSE;

public class PurchaseResponse
{
    
    public long UserId { get; set; }
    public long MovieId { get; set; }
    public DateTime PurchasedAt { get; set; }
    public DateTime? RentalStartDeadline { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
}