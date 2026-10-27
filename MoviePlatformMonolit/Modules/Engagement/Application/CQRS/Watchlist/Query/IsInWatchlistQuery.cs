using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record IsInWatchlistQuery(long UserId, long MovieId) : IRequest<bool>;

public class IsInWatchlistQueryHandler(ApplicationDbContext context)
    : IRequestHandler<IsInWatchlistQuery, bool>
{
    public Task<bool> Handle(IsInWatchlistQuery request, CancellationToken cancellationToken)
    {
        return context.WatchLists
            .AsNoTracking()
            .AnyAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);
    }
}