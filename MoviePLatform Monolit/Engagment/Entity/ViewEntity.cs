using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePLatform_Monolit.Entity;

public class ViewEntity : BaseEntity
{
    public long UserId { get; set; }

    public long MovieId { get; set; }

    public int PositionSeconds { get; set; }
    public bool IsFinished { get; set; }

    public DateTime FirstWatchedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; } = 1;
}