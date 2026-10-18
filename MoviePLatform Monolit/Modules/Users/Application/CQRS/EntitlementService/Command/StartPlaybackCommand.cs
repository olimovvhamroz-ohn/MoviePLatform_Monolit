using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.EntitlementService.Command;

public record StartPlaybackCommand(long UserId, long MovieId) : IRequest<MovieAccessResultResponse>;

public class StartPlaybackCommandHandler(ApplicationDbContext context)
    : IRequestHandler<StartPlaybackCommand, MovieAccessResultResponse>
{
    public async Task<MovieAccessResultResponse> Handle(StartPlaybackCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var user = await context.Users
                       .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                   ?? throw new NotFoundException($"User {request.UserId} not found");

        var movie = await context.Movies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == request.MovieId, cancellationToken)
                    ?? throw new NotFoundException($"Movie {request.MovieId} not found");

        var purchase = await context.Purchases
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);

        var now = DateTime.UtcNow;
        var check = EntitlementEvaluator.CheckAccess(user, movie, purchase, now);

        if (!check.CanWatch)
            return check;

        if (purchase!.FirstWatchedAt == null)
        {
            purchase.FirstWatchedAt = now;
            await context.SaveChangesAsync(cancellationToken);
        }

        return EntitlementEvaluator.CheckAccess(user, movie, purchase, now);
    }
}