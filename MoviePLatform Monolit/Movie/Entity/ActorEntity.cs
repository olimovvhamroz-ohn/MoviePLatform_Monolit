using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class ActorEntity:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; } 

    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
    
}