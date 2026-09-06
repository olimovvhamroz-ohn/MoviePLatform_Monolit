using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class ReviewEntity : BaseEntity
{
    public long UserId { get; set; }

    public long MovieId { get; set; }

    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}