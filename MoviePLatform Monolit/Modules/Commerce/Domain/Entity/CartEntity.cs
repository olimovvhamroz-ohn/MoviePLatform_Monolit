using MoviePLatform_Monolit.Modules.Command.BaseEntity;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

public class CartEntity:BaseEntity
{
    public long UserId { get; set; }

    public bool IsCheckedOut { get; set; }
    public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
}