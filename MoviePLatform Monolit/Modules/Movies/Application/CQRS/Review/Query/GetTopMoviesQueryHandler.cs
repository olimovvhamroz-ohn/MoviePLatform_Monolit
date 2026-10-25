//using System.Text.Json;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetTopMoviesQuery(int Page, int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    //IDistributedCache cache,
    ILogger<GetTopMoviesQueryHandler> logger)
    : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    private const int MaxPageSize = 100;
/*
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };*/

    public async Task<List<MovieResponse>> Handle(GetTopMoviesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new BadRequestException("Page and pageSize must be greater than 0");

        var pageSize = Math.Min(request.PageSize, MaxPageSize);
        /*   var cacheKey = $"movie:top:page:{request.Page}:size:{pageSize}";

           var cached = await cache.GetStringAsync(cacheKey, cancellationToken);
           if (!string.IsNullOrEmpty(cached))
           {
               var cachedResponse = JsonSerializer.Deserialize<List<MovieResponse>>(cached);
               if (cachedResponse is not null)
               {
                   logger.LogInformation("Top movies page {Page} served from cache", request.Page);
                   return cachedResponse;
               }
           }*/

        logger.LogInformation("Getting top movies page {Page} from database", request.Page);

        var skip = (request.Page - 1) * pageSize;

        var movies = await context.Movies
            .AsNoTracking()
            .Where(x => x.IsDeleted == false)
            .Include(m => m.Category)
            .Where(m => context.Reviews.Any(r => r.MovieId == m.Id))
            .OrderByDescending(m => context.Reviews
                .Where(r => r.MovieId == m.Id)
                .Average(r => (double?)r.Rating))
            .ThenBy(m => m.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var response = mapper.Map<List<MovieResponse>>(movies);

        /*await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            CacheOptions,
            cancellationToken);*/

        return response;
    }
}