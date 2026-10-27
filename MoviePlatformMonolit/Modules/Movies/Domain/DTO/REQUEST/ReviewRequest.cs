namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;

public class ReviewRequest
{
    public long MovieId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}