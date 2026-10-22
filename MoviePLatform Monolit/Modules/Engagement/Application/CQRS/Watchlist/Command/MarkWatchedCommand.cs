using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Command;

public record MarkWatchedCommand(long UserId, long MovieId) : IRequest<WatchlistResponse>;

public class MarkWatchedCommandHandler(
    ApplicationDbContext context,
    ILogger<MarkWatchedCommandHandler> logger) : IRequestHandler<MarkWatchedCommand, WatchlistResponse>
{
    public async Task<WatchlistResponse> Handle(MarkWatchedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking watched: user {UserId}, movie {MovieId}", request.UserId, request.MovieId);

        var entity = await context.Watchlists
                         .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken)
                     ?? throw new NotFoundException("Watchlist entry not found");

        entity.IsWatched = true;
        await context.SaveChangesAsync(cancellationToken);

        return new WatchlistResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            MovieId = entity.MovieId,
            IsWatched = entity.IsWatched,
            AddedAt = entity.AddedAt
        };
    }
}