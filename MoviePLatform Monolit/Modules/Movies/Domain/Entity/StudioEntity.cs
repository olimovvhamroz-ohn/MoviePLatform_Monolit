using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class StudioEntity:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FoundYear { get; set; }

    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}