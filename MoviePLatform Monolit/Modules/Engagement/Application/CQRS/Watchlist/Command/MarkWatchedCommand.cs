

using MediatR;

public record MarkWatchedCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class MarkWatchedCommandHandler : IRequestHandler<MarkWatchedCommand, WatchlistResponse>
{
    private readonly IWatchlistRepository _repository;
    private readonly ILogger<MarkWatchedCommandHandler> _logger;

    public MarkWatchedCommandHandler(IWatchlistRepository repository, ILogger<MarkWatchedCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<WatchlistResponse> Handle(MarkWatchedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking watched: user {UserId}, movie {MovieId}", request.UserId, request.MovieId);

        var updated = await _repository.MarkWatched(request.UserId, request.MovieId);
        return new WatchlistResponse
        {
            Id = updated.Id,
            UserId = updated.UserId,
            MovieId = updated.MovieId,
            IsWatched = updated.IsWatched,
            AddedAt = updated.AddedAt
        };
    }
}