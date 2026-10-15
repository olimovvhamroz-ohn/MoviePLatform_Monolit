using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;
public class WatchlistEntity:BaseEntity
{
    public long UserId { get; set; }
    public long  MovieId { get; set; }
    public bool  IsWatched { get; set; }
    public DateTime AddedAt { get; set; }
    public MovieEntity? Movie { get; set; }
}