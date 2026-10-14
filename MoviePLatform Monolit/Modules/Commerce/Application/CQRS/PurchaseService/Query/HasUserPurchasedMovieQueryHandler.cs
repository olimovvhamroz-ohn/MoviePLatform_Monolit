using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record HasUserPurchasedMovieQuery(long UserId, long MovieId) : IRequest<bool>;

public class HasUserPurchasedMovieQueryHandler(IPurchaseRepository repository) : IRequestHandler<HasUserPurchasedMovieQuery, bool>
{
 

    public async Task<bool> Handle(HasUserPurchasedMovieQuery request, CancellationToken cancellationToken)
    {
        return await repository.HasUserPurchasedMovie(request.UserId, request.MovieId);
    }
}