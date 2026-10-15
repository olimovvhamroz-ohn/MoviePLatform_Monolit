using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

public class UserFavoriteCategoryEntity:BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public long CategoryId { get; set; }
}