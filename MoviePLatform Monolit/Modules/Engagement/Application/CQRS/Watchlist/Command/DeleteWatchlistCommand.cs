using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record DeleteWatchlistCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class DeleteWatchlistCommandHandler(
    ApplicationDbContext context,
    ILogger<DeleteWatchlistCommandHandler> logger) : IRequestHandler<DeleteWatchlistCommand, WatchlistResponse>
{
    public async Task<WatchlistResponse> Handle(DeleteWatchlistCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing movie {MovieId} from watchlist for user {UserId}", request.MovieId, request.UserId);

        var entity = await context.Watchlists
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

        context.Watchlists.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return response;
    }
}