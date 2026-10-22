using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Watchlist.Query;

public record GetCountByUserId(long Id) : IRequest<int>;

public class GetCountByUserIdQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetCountByUserId, int>
{
    public Task<int> Handle(GetCountByUserId request, CancellationToken cancellationToken)
    {
        return context.Watchlists
            .AsNoTracking()
            .CountAsync(x => x.UserId == request.Id, cancellationToken);
    }
}