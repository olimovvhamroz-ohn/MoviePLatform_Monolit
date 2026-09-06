
using MoviePLatform_Monolit.Entity;

public class WatchlistEntity:BaseEntity
{
    public long UserId { get; set; }
    public long  MovieId { get; set; }
    public bool  IsWatched { get; set; }
    public DateTime AddedAt { get; set; }
    public MovieEntity? Movie { get; set; }
}