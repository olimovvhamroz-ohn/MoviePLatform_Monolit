using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record GetWatchlistByUserIdQuery(long UserId) : IRequest<List<WatchlistResponse>>;

public class GetWatchlistByUserIdQueryHandler(IWatchlistRepository repository) : IRequestHandler<GetWatchlistByUserIdQuery, List<WatchlistResponse>>
{
    
    public async Task<List<WatchlistResponse>> Handle(GetWatchlistByUserIdQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetByUserId(request.UserId);
        return items.Select(x => new WatchlistResponse
        {
            Id = x.Id,
            UserId = x.UserId,
            MovieId = x.MovieId,
            MovieTitle = x.Movie?.Title,
            PosterUrl = x.Movie?.PosterUrl,
            IsWatched = x.IsWatched,
            AddedAt = x.AddedAt
        }).ToList();
    }
}