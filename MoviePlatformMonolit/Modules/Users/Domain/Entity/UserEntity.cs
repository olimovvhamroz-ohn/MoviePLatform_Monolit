using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;
using MoviePlatformMonolit.Modules.Users.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Users.Domain.Entity;

public class UserEntity:BaseEntity
{
    
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.Client;
    public DateTime? DateOfBirth { get; set; }
  
    public ICollection<UserFavoriteCategoryEntity> FavoriteCategories { get; set; } = new List<UserFavoriteCategoryEntity>();
}