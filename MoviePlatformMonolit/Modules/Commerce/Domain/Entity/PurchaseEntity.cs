using MoviePlatformMonolit.Modules.Command.BaseEntity;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;
using MoviePlatformMonolit.Modules.Users.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Entity;

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