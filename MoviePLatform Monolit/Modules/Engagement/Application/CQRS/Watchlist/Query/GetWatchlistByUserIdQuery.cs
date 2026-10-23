using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record GetWatchlistByUserIdQuery(long UserId) : IRequest<List<WatchlistResponse>>;

public class GetWatchlistByUserIdQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetWatchlistByUserIdQuery, List<WatchlistResponse>>
{
    public async Task<List<WatchlistResponse>> Handle(GetWatchlistByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Watchlists
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.AddedAt)
            .ThenByDescending(x => x.Id)
            .Select(x => new WatchlistResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                MovieId = x.MovieId,
                MovieTitle = x.Movie!.Title,
                PosterUrl = x.Movie.PosterUrl,
                IsWatched = x.IsWatched,
                AddedAt = x.AddedAt
            })
            .ToListAsync(cancellationToken);
    }
}