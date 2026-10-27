using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Entity;

public class CartItemEntity:BaseEntity
{
    public long CartId { get; set; }
    public CartEntity Cart { get; set; } = null!;

    public long MovieId { get; set; }
}