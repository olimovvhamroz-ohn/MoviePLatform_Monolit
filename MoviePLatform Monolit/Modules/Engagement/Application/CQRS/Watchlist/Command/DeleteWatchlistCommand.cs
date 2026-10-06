
using MediatR;

public record DeleteWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class DeleteWatchlistCommandHandler : IRequestHandler<DeleteWatchlistCommand, WatchlistResponse>
{
    private readonly IWatchlistRepository _repository;
    private readonly ILogger<DeleteWatchlistCommandHandler> _logger;

    public DeleteWatchlistCommandHandler(IWatchlistRepository repository, ILogger<DeleteWatchlistCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<WatchlistResponse> Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", request.MovieId, request.UserId);

        var deleted = await _repository.Delete(request.UserId, request.MovieId);
        return new WatchlistResponse
        {
            Id = deleted.Id,
            UserId = deleted.UserId,
            MovieId = deleted.MovieId,
            IsWatched = deleted.IsWatched,
            AddedAt = deleted.AddedAt
        };
    }
}