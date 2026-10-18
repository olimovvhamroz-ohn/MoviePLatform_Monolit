using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

public record HasUserPurchasedMovieQuery(long UserId, long MovieId) : IRequest<bool>;

public class HasUserPurchasedMovieQueryHandler(ApplicationDbContext context)
    : IRequestHandler<HasUserPurchasedMovieQuery, bool>
{
    public Task<bool> Handle(HasUserPurchasedMovieQuery request, CancellationToken cancellationToken)
    {
        return context.Purchases
            .AnyAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);
    }
}