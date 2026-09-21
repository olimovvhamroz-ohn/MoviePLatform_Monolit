using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

public class ActorEntity:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; } 

    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
    
}