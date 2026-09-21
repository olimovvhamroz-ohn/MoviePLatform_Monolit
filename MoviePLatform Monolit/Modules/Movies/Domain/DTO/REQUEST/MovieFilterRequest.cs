using MoviePLatform_Monolit.Movie.Entity.Enums;

namespace MoviePLatform_Monolit.Movie.DTO.REQUEST;

public class MovieFilterRequest
{
    public long? CategoryId { get; set; }
    public int? Year { get; set; }
    public decimal? MaxPrice { get; set; }
    public AgeRating? AgeRating { get; set; }
}