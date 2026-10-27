using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;

namespace MoviePlatformMonolit.Modules.Commerce.Application.CQRS.PurchaseService.Query;

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