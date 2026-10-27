namespace MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

public class ReviewResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}