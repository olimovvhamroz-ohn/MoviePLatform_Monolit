

public class ViewResponse
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public string? PosterUrl { get; set; }
    public int PositionSeconds { get; set; }
    public bool IsCompleted { get; set; }
    public int ViewCount { get; set; }
    public DateTime LastWatchedAt { get; set; }
}

