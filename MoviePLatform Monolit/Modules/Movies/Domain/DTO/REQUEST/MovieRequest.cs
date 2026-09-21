using MoviePLatform_Monolit.Movie.Entity.Enums;

namespace MoviePLatform_Monolit.Movie.DTO.REQUEST;

public class MovieRequest
{
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public TimeSpan Duration { get; set; }
    public AgeRating AgeRating { get; set; } = AgeRating.G; 
    
    public long CategoryId { get; set; }
    public long? StudioId { get; set; }

    public List<long> ActorIds { get; set; } = new();
}