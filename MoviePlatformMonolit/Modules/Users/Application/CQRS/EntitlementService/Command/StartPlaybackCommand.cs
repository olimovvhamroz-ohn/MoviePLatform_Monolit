using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Users.Application.CQRS.EntitlementService.Command;

public record StartPlaybackCommand(long UserId, long MovieId) : IRequest<MovieAccessResultResponse>;

public class StartPlaybackCommandHandler(ApplicationDbContext context)
    : IRequestHandler<StartPlaybackCommand, MovieAccessResultResponse>
{
    public async Task<MovieAccessResultResponse> Handle(
        StartPlaybackCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"User {request.UserId} not found");

        var movie = await context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.MovieId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Movie {request.MovieId} not found");

        var purchase = await context.Purchases
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.MovieId == request.MovieId, cancellationToken);

        var now = DateTime.UtcNow;
        var check = EntitlementEvaluator.CheckAccess(user, movie, purchase, now);

        if (!check.CanWatch || purchase is null)
            return check;

        if (purchase.FirstWatchedAt is null)
        {
            var updated = await context.Purchases
                .Where(x => x.Id == purchase.Id && x.FirstWatchedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.FirstWatchedAt, now), cancellationToken);

            if (updated == 0)
                await context.Entry(purchase).ReloadAsync(cancellationToken);
            else
                purchase.FirstWatchedAt = now;
        }

        return EntitlementEvaluator.CheckAccess(user, movie, purchase, now);
    }
}