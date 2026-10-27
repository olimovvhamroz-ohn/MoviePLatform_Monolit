using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;

namespace MoviePlatformMonolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record GetCountByUserId(long Id) : IRequest<int>;

public class GetCountByUserIdQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetCountByUserId, int>
{
    public Task<int> Handle(GetCountByUserId request, CancellationToken cancellationToken)
    {
        return context.WatchLists
            .AsNoTracking()
            .CountAsync(x => x.UserId == request.Id, cancellationToken);
    }
}