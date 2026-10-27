using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record DeleteWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class DeleteWatchlistCommandHandler(
    ApplicationDbContext context,
    ILogger<DeleteWatchlistCommandHandler> logger) : IRequestHandler<DeleteWatchlistCommand, WatchlistResponse>
{
    public async Task<WatchlistResponse> Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", request.MovieId, request.UserId);

        var entity = await context.WatchLists
                         .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken)
                     ?? throw new NotFoundException("Watchlist entry not found");

        var response = new WatchlistResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MovieId = entity.MovieId,
            IsWatched = entity.IsWatched,
            AddedAt = entity.AddedAt
        };

        context.WatchLists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return response;
    }
}