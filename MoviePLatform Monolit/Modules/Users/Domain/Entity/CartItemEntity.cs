using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class CartItemEntity:BaseEntity
{
    public long CartId { get; set; }
    public CartEntity Cart { get; set; } = null!;

    public long MovieId { get; set; }
}