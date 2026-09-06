

public interface IWatchlistRepository
{
    Task<List<WatchlistEntity>> GetByUserId(long userId);
    Task<WatchlistEntity> Create(WatchlistEntity entity);
    Task<WatchlistEntity> MarkWatched(long userId, long movieId);
    Task<bool> Exists(long userId, long movieId);
    Task<WatchlistEntity> Delete(long userId, long movieId);
}