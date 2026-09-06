using MoviePLatform_Monolit.Movie.Entity;

namespace MoviePlatform_Monolit.Engagment.Entity;

public class WatchlistEntity : BaseEntity
{
    public long UserId { get; set; }

    public long MovieId { get; set; }

    public bool IsWatched { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}