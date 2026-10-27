using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Entity;

public class StudioEntity:BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int FoundYear { get; set; }

    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}