using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record DeleteWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class DeleteWatchlistCommandHandler(IWatchlistRepository repository, ILogger<DeleteWatchlistCommandHandler> logger) : IRequestHandler<DeleteWatchlistCommand, WatchlistResponse>
{
   

    public async Task<WatchlistResponse> Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", request.MovieId, request.UserId);

        var deleted = await repository.Delete(request.UserId, request.MovieId);
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