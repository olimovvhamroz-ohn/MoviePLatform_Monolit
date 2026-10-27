using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
using MoviePlatformMonolit.Modules.Command.BaseEntity;

namespace MoviePlatformMonolit.Modules.Movies.Domain.Entity;

public class CategoryEntity:BaseEntity
{
    public  string Title { get; set; } = string.Empty;
    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}