
using MediatR;

public record IsInWatchlistQuery(long UserId, long MovieId) : IRequest<bool>;

public class IsInWatchlistQueryHandler : IRequestHandler<IsInWatchlistQuery, bool>
{
    private readonly IWatchlistRepository _repository;

    public IsInWatchlistQueryHandler(IWatchlistRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(IsInWatchlistQuery request, CancellationToken cancellationToken)
    {
        return _repository.Exists(request.UserId, request.MovieId);
    }
}