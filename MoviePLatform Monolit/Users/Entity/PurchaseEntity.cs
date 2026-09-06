using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class PurchaseEntity : BaseEntity
{
    public long UserId { get; set; }

    public long MovieId { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FirstWatchedAt { get; set; }
}