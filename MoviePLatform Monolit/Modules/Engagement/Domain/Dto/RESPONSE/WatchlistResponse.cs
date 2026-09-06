

public class WatchlistResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public string? PosterUrl { get; set; }
    public bool IsWatched { get; set; }
    public DateTime AddedAt { get; set; }
}