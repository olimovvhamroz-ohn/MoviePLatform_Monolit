using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetTopMoviesQuery(
    int Page,
    int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler(
    IReviewRepository reviewRepository,
    IMapper mapper,
    ILogger<GetTopMoviesQueryHandler> logger,
    IDistributedCache cache) : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetTopMoviesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException(
                "Page and pageSize must be greater than 0");
        }

        var key =
            $"movie:top:page:{request.Page}:size:{request.PageSize}";

        // 1. Redis
        var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Top movies page {Page} size {PageSize} found in Redis",
                request.Page,
                request.PageSize);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. PostgreSQL
        logger.LogInformation(
            "Getting top movies page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);

        var movies = await reviewRepository.GetTopMovie(
            request.Page,
            request.PageSize);

        // 3. Map
        var response = mapper.Map<List<MovieResponse>>(movies);

        // 4. Serialize
        var json = JsonSerializer.Serialize(response);

        // 5. Redis
        await cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return response;
    }
}