using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;

public record GetHistoryQuery(long UserId, int Page, int PageSize) : IRequest<List<ViewResponse>>;

public class GetHistoryQueryHandler(IViewRepository repository) : IRequestHandler<GetHistoryQuery, List<ViewResponse>>
{
    

    public async Task<List<ViewResponse>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and pageSize must be greater than 0");

        var items = await repository.GetHistory(request.UserId, request.Page, request.PageSize);
        return items.Select(x => new ViewResponse
        {
            Id = x.Id,
            UserId = x.UserId,
            MovieId = x.MovieId,
            MovieTitle = x.Movie?.Title,
            PosterUrl = x.Movie?.PosterUrl,
            PositionSeconds = x.PositionSeconds,
            IsCompleted = x.IsCompleted,
            ViewCount = x.ViewCount,
            LastWatchedAt = x.LastWatchedAt
        }).ToList();
    }
}