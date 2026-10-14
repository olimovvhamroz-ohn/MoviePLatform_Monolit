

public interface IViewRepository
{
    Task<ViewEntity> RecordView(long userId, long movieId, int positionSeconds, bool isCompleted);

    Task<List<ViewEntity>> GetHistory(long userId, int page, int pageSize);

    Task<List<ViewEntity>> GetContinueWatching(long userId);

    Task<List<(long MovieId, int Views)>> GetTrending(int days, int take);
}