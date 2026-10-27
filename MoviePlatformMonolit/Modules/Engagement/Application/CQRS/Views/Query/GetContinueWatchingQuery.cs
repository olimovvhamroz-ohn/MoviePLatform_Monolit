using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Views.Query;

public record GetContinueWatchingQuery(long UserId, int Take = 20) : IRequest<List<ViewResponse>>;

public class GetContinueWatchingQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetContinueWatchingQuery, List<ViewResponse>>
{
    private const int MaxTake = 50;

    public async Task<List<ViewResponse>> Handle(GetContinueWatchingQuery request, CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, MaxTake);

        return await context.Views
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId
                        && !x.IsCompleted
                        && x.PositionSeconds > 0)
            .OrderByDescending(x => x.LastWatchedAt)
            .Take(take)
            .Select(x => new ViewResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                MovieId = x.MovieId,
                MovieTitle = x.Movie!.Title,
                PosterUrl = x.Movie.PosterUrl,
                PositionSeconds = x.PositionSeconds,
                IsCompleted = x.IsCompleted,
                ViewCount = x.ViewCount,
                LastWatchedAt = x.LastWatchedAt
            })
            .ToListAsync(cancellationToken);
    }
}