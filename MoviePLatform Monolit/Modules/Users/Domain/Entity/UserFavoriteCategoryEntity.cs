using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class UserFavoriteCategoryEntity:BaseEntity
{
    public long UserId { get; set; }
    public UserEntity User { get; set; }

    public long CategoryId { get; set; }
}