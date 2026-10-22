using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetTopMoviesQuery(int Page, int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<GetTopMoviesQueryHandler> logger,
    IDistributedCache cache) : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(GetTopMoviesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and pageSize must be greater than 0");

        var pageSize = Math.Min(request.PageSize, 100);
        var key = $"movie:top:page:{request.Page}:size:{pageSize}";

        var cached = await cache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<List<MovieResponse>>(cached);
            if (cachedResponse is not null)
            {
                logger.LogInformation("Top movies page {Page} served from cache", request.Page);
                return cachedResponse;
            }
        }

        logger.LogInformation("Getting top movies page {Page} from database", request.Page);

        var skip = (request.Page - 1) * pageSize;

        var movies = await context.Reviews
            .AsNoTracking()
            .GroupBy(r => r.MovieId)
            .Select(g => new { MovieId = g.Key, Avg = g.Average(r => r.Rating) })
            .OrderByDescending(x => x.Avg)
            .Skip(skip)
            .Take(pageSize)
            .Join(context.Movies.Include(m => m.Category),
                  top => top.MovieId,
                  m => m.Id,
                  (top, m) => new { top.Avg, Movie = m })
            .OrderByDescending(x => x.Avg)
            .Select(x => x.Movie)
            .ToListAsync(cancellationToken);

        var response = mapper.Map<List<MovieResponse>>(movies);

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
            cancellationToken);

        return response;
    }
}