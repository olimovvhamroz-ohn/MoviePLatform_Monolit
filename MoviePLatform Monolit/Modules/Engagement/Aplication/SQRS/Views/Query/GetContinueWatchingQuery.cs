

using MediatR;

public record GetContinueWatchingQuery(long UserId) : IRequest<List<ViewResponse>>;

public class GetContinueWatchingQueryHandler : IRequestHandler<GetContinueWatchingQuery, List<ViewResponse>>
{
    private readonly IViewRepository _repository;

    public GetContinueWatchingQueryHandler(IViewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ViewResponse>> Handle(GetContinueWatchingQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetContinueWatching(request.UserId);
        return items.Select(x => new ViewResponse
        {
            Id = x.Id,
            UserId = x.UserId,
            MovieId = x.MovieId,
            MovieTitle = x.Movie?.Title,
            PosterUrl = x.Movie?.PosterUrl,
            PositionSeconds = x.PositionSeconds,
            IsCompleted = x.IsCompleted,
            ViewCount = x.ViewCount,
            LastWatchedAt = x.LastWatchedAt
        }).ToList();
    }
}