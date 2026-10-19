using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;

public record GetHistoryQuery(long UserId, int Page, int PageSize) : IRequest<List<ViewResponse>>;

public class GetHistoryQueryHandler(ApplicationDbContext context) : IRequestHandler<GetHistoryQuery, List<ViewResponse>>
{
    private const int MaxPrice=50;


    public async Task<List<ViewResponse>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            throw new BadRequestException("User ID must be greater than zero");

            
        }

        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException("Page and pageSize must be greater than 0");
        }

        var pageSize = Math.Min(request.PageSize, MaxPrice);
        var skip = (request.Page - 1) * pageSize;

        return await context.Views
            .Include(x => x.Movie)
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.LastWatchedAt)
            .ThenByDescending(x => x.Id)
            .Skip(skip)
            .Take(pageSize).Select(x => new ViewResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                MovieId = x.MovieId,
                MovieTitle = x.Movie.Title,
                PosterUrl = x.Movie.PosterUrl,
                PositionSeconds = x.PositionSeconds,
                IsCompleted = x.IsCompleted,
                ViewCount = x.ViewCount,
                LastWatchedAt = x.LastWatchedAt
            })
            .ToListAsync(cancellationToken);
    }
}
