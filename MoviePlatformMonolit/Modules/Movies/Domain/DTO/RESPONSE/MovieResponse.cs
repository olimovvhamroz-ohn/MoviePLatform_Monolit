using MoviePlatformMonolit.Modules.Movies.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

public class MovieResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public TimeSpan Duration { get; set; }
    public AgeRating AgeRating { get; set; } = AgeRating.G;

    public CategoryResponse? Category { get; set; }
    public StudioResponse? Studio { get; set; }
    public ICollection<ActorResponse> Actors { get; set; } = new List<ActorResponse>();
    public ICollection<ReviewResponse> Reviews { get; set; } = new List<ReviewResponse>();
}