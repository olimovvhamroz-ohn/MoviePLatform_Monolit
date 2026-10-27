using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetMovieReviewsQuery(long MovieId, int Page, int PageSize)
    : IRequest<List<ReviewResponse>>;

public class GetMovieReviewQueryHandler(ApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetMovieReviewsQuery, List<ReviewResponse>>
{
    public async Task<List<ReviewResponse>> Handle(GetMovieReviewsQuery request, CancellationToken cancellationToken)
    {
        if (request.MovieId <= 0)
            throw new BadRequestException("Movie ID must be greater than zero");

        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and pageSize must be greater than 0");

        var pageSize = Math.Min(request.PageSize, 100);
        var skip = (int)Math.Min((long)(request.Page - 1) * pageSize, int.MaxValue);

        var reviews = await context.Reviews
            .AsNoTracking()
            .Where(x => x.IsDeleted == false)
            .Where(x => x.MovieId == request.MovieId)
            .OrderByDescending(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<ReviewResponse>>(reviews);
    }
}