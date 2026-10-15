using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record GetCountByUserId(long Id) : IRequest<int>;

public class GetCountByIdQuery(IWatchlistRepository watchlistRepository) : IRequestHandler<GetCountByUserId, int>
{
   


    public async Task<int> Handle(
        GetCountByUserId request,
        CancellationToken cancellationToken)
    {
        var count = await watchlistRepository.CountByUserId(request.Id);

        return count;
    }
}