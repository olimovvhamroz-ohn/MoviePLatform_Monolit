using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesPaginationQuery(int Page, int PageSize)
    : IRequest<PaginatedResponse<MovieResponse>>;

public class GetMoviesPaginationQueryHandler(
    ApplicationDbContext context,
    ILogger<GetMoviesPaginationQueryHandler> logger)
    : IRequestHandler<GetMoviesPaginationQuery, PaginatedResponse<MovieResponse>>
{
    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 100;

    public async Task<PaginatedResponse<MovieResponse>> Handle(
        GetMoviesPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = request.PageSize <= 0
            ? DefaultPageSize
            : Math.Min(request.PageSize, MaxPageSize);

        var skip = (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);

        logger.LogInformation("Getting movies page {Page} size {PageSize}", page, pageSize);

        var query = context.Movies
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(MovieProjections.ToResponse)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<MovieResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        };
    }
}