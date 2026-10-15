using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record MarkWatchedCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class MarkWatchedCommandHandler (IWatchlistRepository repository, ILogger<MarkWatchedCommandHandler> logger): IRequestHandler<MarkWatchedCommand, WatchlistResponse>
{
   

    public async Task<WatchlistResponse> Handle(MarkWatchedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking watched: user {UserId}, movie {MovieId}", request.UserId, request.MovieId);

        var updated = await repository.MarkWatched(request.UserId, request.MovieId);
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