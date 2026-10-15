
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public interface IWatchlistRepository
{
    Task<List<WatchlistEntity>> GetByUserId(long userId);
    Task<WatchlistEntity> Create(WatchlistEntity entity);
    Task<WatchlistEntity> MarkWatched(long userId, long movieId);
    Task<bool> Exists(long userId, long movieId);
    Task<WatchlistEntity> Delete(long userId, long movieId);
    Task<int>CountByUserId(long userId);
}