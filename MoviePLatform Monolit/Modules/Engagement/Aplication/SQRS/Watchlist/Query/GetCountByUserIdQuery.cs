using MediatR;

namespace MoviePLatform_Monolit.Modules.Engagement.Aplication.SQRS.Watchlist.Query;

public record GetCountByUserId(long Id) : IRequest<int>;

public class GetCountByIdQuery : IRequestHandler<GetCountByUserId, int>
{
    private readonly IWatchlistRepository _watchlistRepository;

    public GetCountByIdQuery(IWatchlistRepository repository)
    {
        _watchlistRepository = repository;
    }


    public async Task<int> Handle(
        GetCountByUserId request,
        CancellationToken cancellationToken)
    {
        var count = await _watchlistRepository.CountByUserId(request.Id);

        return count;
    }
}