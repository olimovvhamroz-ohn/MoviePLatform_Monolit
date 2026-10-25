using MoviePLatform_Monolit.Modules.Command.BaseEntity;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

public class StudioEntity:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FoundYear { get; set; }

    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}