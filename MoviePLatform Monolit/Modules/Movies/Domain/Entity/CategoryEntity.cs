using MoviePLatform_Monolit.Modules.Command.BaseEntity;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

public partial class CategoryEntity:BaseEntity
{
    public  string Title { get; set; } = string.Empty;
    public ICollection<MovieEntity> Movies { get; set; } = new List<MovieEntity>();
}