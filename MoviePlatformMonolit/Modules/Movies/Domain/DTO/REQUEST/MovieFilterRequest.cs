using MoviePlatformMonolit.Modules.Movies.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
public class MovieFilterRequest
{
    public long? CategoryId { get; set; }
    public int? Year { get; set; }
    public decimal? MaxPrice { get; set; }
    public AgeRating? AgeRating { get; set; }
}