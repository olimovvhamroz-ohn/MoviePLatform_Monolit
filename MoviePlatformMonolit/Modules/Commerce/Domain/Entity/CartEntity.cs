using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;

namespace MoviePlatformMonolit.Modules.Commerce.Domain.Entity;

public class CartEntity:BaseEntity
{
    public long UserId { get; set; }

    public bool IsCheckedOut { get; set; }
    public ICollection<CartItemEntity> CartItems { get; set; } = new List<CartItemEntity>();
}