using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record IsInWatchlistQuery(long UserId, long MovieId) : IRequest<bool>;

public class IsInWatchlistQueryHandler (IWatchlistRepository repository): IRequestHandler<IsInWatchlistQuery, bool>
{
   
    public Task<bool> Handle(IsInWatchlistQuery request, CancellationToken cancellationToken)
    {
        return repository.Exists(request.UserId, request.MovieId);
    }
}