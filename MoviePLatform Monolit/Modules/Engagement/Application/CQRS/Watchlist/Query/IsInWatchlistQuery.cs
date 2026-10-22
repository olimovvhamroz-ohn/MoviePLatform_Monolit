using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record IsInWatchlistQuery(long UserId, long MovieId) : IRequest<bool>;

public class IsInWatchlistQueryHandler(ApplicationDbContext context)
    : IRequestHandler<IsInWatchlistQuery, bool>
{
    public Task<bool> Handle(IsInWatchlistQuery request, CancellationToken cancellationToken)
    {
        return context.Watchlists
            .AsNoTracking()
            .AnyAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);
    }
}