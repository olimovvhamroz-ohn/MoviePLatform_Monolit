using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;

namespace MoviePlatformMonolit.Modules.Users.Domain.Entity;

public class UserFavoriteCategoryEntity:BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public long CategoryId { get; set; }
}