using MediatR;

;

public record HasUserPurchasedMovieQuery(long UserId, long MovieId) : IRequest<bool>;

public class HasUserPurchasedMovieQueryHandler : IRequestHandler<HasUserPurchasedMovieQuery, bool>
{
    private readonly IPurchaseRepository _repository;

    public HasUserPurchasedMovieQueryHandler(IPurchaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(HasUserPurchasedMovieQuery request, CancellationToken cancellationToken)
    {
        return await _repository.HasUserPurchasedMovie(request.UserId, request.MovieId);
    }
}