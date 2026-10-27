using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetTopMoviesQuery(int Page, int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler(
    ApplicationDbContext context,
    ILogger<GetTopMoviesQueryHandler> logger)
    : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    private const int MaxPageSize = 100;

    public async Task<List<MovieResponse>> Handle(GetTopMoviesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and pageSize must be greater than 0");

        var pageSize = Math.Min(request.PageSize, MaxPageSize);
        var skip = (int)Math.Min((long)(request.Page - 1) * pageSize, int.MaxValue);

        logger.LogInformation("Getting top movies page {Page}", request.Page);

        return await context.Movies
            .AsNoTracking()
            .Where(m => !m.IsDeleted && m.Reviews.Any(r => !r.IsDeleted))
            .OrderByDescending(m => m.Reviews
                .Where(r => !r.IsDeleted)
                .Average(r => (double?)r.Rating))
            .ThenBy(m => m.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(MovieProjections.ToResponse)
            .ToListAsync(cancellationToken);
    }
}