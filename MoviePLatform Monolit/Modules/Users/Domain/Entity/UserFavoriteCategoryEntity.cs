public class UserFavoriteCategoryEntity:BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; }

    public long CategoryId { get; set; }
}