

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

public class TrendingMovieResponse
{
    public long MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public string? PosterUrl { get; set; }
    public int Views { get; set; }
}