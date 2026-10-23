using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

public class CartItemEntity:BaseEntity
{
    public long CartId { get; set; }
    public CartEntity Cart { get; set; } = null!;

    public long MovieId { get; set; }
}