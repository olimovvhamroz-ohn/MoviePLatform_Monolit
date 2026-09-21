using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Modules.Users.Domain.Entity;

public class CartEntity:BaseEntity
{
    public long UserId { get; set; }

    public bool IsCheckedOut { get; set; }
    public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
}