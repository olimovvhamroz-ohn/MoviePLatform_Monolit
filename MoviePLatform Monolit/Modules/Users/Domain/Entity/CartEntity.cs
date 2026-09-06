using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class CartEntity:BaseEntity
{
    public long UserId { get; set; }

    public bool IsCheckedOut { get; set; }
    public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
}