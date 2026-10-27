using MoviePlatformMonolit.Modules.Command.BaseEntity;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Engagement.Domain.Entities;
public class WatchListEntity:BaseEntity
{
    public long UserId { get; set; }
    public long  MovieId { get; set; }
    public bool  IsWatched { get; set; }
    public DateTime AddedAt { get; set; }
    public MovieEntity? Movie { get; set; }
}