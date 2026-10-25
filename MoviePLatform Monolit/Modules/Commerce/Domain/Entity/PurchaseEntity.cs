using MoviePLatform_Monolit.Modules.Command.BaseEntity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

public class PurchaseEntity : BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; } = null!; 

    public long MovieId { get; set; }
    public MovieEntity Movie { get; set; } = null!; 
    
    public decimal PurchasePrice { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FirstWatchedAt { get; set; }
}