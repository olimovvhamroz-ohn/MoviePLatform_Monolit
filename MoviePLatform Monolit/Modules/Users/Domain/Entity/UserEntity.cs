using MoviePLatform_Monolit.Enums;
using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

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