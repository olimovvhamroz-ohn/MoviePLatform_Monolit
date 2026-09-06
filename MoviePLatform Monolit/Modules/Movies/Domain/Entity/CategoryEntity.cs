using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class CategoryEntity:BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}