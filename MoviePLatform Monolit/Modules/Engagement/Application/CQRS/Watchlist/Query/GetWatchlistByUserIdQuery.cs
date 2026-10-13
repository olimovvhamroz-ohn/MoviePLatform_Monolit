

using MediatR;

public record GetWatchlistByUserIdQuery(long UserId) : IRequest<List<WatchlistResponse>>;

public class GetWatchlistByUserIdQueryHandler : IRequestHandler<GetWatchlistByUserIdQuery, List<WatchlistResponse>>
{
    private readonly IWatchlistRepository _repository;

    public GetWatchlistByUserIdQueryHandler(IWatchlistRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WatchlistResponse>> Handle(GetWatchlistByUserIdQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetByUserId(request.UserId);
        return items.Select(x => new WatchlistResponse
        {
            Id = x.Id,
            UserId = x.UserId,
            MovieId = x.MovieId,
            MovieTitle = x.Movie?.Title,
            PosterUrl = x.Movie?.PosterUrl,
            IsWatched = x.IsWatched,
            AddedAt = x.AddedAt
        }).ToList();
    }
}