using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Movie.Entity.Enums;

namespace MoviePLatform_Monolit.Movie.DTO.RESPONSE;

public class MovieResponse
{
    public long Id  { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public TimeSpan Duration { get; set; }
    public AgeRating AgeRating { get; set; } = AgeRating.G;

    public CategoryResponse? Category { get; set; }

    public StudioEntity? Studio { get; set; }

    public ICollection<ActorEntity> Actors { get; set; } = new List<ActorEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new List<ReviewEntity>();
}