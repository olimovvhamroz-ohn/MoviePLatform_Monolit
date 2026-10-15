using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Dto.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Engagement.Application.CQRS.Views.Query;

public  record GetContinueWatchingQuery(long UserId) : IRequest<List<ViewResponse>>;

public class GetContinueWatchingQueryHandler(IViewRepository repository) : IRequestHandler<GetContinueWatchingQuery, List<ViewResponse>>
{
    
    public async Task<List<ViewResponse>> Handle(GetContinueWatchingQuery request, CancellationToken cancellationToken)
    {
        var items = await repository.GetContinueWatching(request.UserId);
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