using MoviePLatform_Monolit.Movie.Entity;
using MoviePLatform_Monolit.Movie.Entity.Enums;

namespace MoviePLatform_Monolit.Entity;

public class MovieEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public TimeSpan Duration { get; set; }
    public AgeRating AgeRating { get; set; } = AgeRating.G;

    public long? CategoryId { get; set; }
    public CategoryEntity? Category { get; set; }

    public long? StudioId { get; set; }
    public StudioEntity? Studio { get; set; }

    public ICollection<ActorEntity> Actors { get; set; } = new List<ActorEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
}