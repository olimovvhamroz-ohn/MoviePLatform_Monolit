namespace MoviePLatform_Monolit.Movie.DTO.RESPONSE;

public class ReviewRespose
{
    public long UserId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}