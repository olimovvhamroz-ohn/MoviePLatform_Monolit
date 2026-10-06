using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Modules.Movies.Aplication.CQRS.Review.Query;

public record GetTopMoviesQuery(
    int Page,
    int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler
    : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTopMoviesQueryHandler> _logger;
    private readonly IDistributedCache _cache;

    public GetTopMoviesQueryHandler(
        IReviewRepository reviewRepository,
        IMapper mapper,
        ILogger<GetTopMoviesQueryHandler> logger,
        IDistributedCache cache)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

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
        var cachedMovies = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            _logger.LogInformation(
                "Top movies page {Page} size {PageSize} found in Redis",
                request.Page,
                request.PageSize);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. PostgreSQL
        _logger.LogInformation(
            "Getting top movies page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);

        var movies = await _reviewRepository.GetTopMovie(
            request.Page,
            request.PageSize);

        // 3. Map
        var response = _mapper.Map<List<MovieResponse>>(movies);

        // 4. Serialize
        var json = JsonSerializer.Serialize(response);

        // 5. Redis
        await _cache.SetStringAsync(
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