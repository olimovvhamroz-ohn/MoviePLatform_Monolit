using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesPaginationQuery(
    int Page,
    int PageSize) : IRequest<List<MovieResponse>>;

public class GetMoviesPaginationQueryHandler(
    IMovieRepository repository,
    IMapper mapper,
    ILogger<GetMoviesPaginationQueryHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<GetMoviesPaginationQuery, List<MovieResponse>>
{
   

    public async Task<List<MovieResponse>> Handle(
        GetMoviesPaginationQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException(
                "Page and PageSize must be greater than 0");
        }

        var key =
            $"movie:page:{request.Page}:size:{request.PageSize}";

        var cachedMovies = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            logger.LogInformation(
                "Movies page {Page} size {PageSize} found in Redis",
                request.Page,
                request.PageSize);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        logger.LogInformation(
            "Getting movies page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);

        var movies = await repository.PaginationAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        // 3. Entity → Response
        var response = mapper.Map<List<MovieResponse>>(movies);

        // 4. Response → JSON
        var json = JsonSerializer.Serialize(response);

        // 5. Save to Redis
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