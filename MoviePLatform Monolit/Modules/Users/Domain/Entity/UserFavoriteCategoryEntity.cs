public class UserFavoriteCategoryEntity:BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public long CategoryId { get; set; }
}