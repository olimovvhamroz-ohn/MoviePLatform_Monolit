using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record GetMoviesPaginationQuery(
    int Page,
    int PageSize) : IRequest<List<MovieResponse>>;

public class GetMoviesPaginationQueryHandler
    : IRequestHandler<GetMoviesPaginationQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMoviesPaginationQueryHandler> _logger;
    private readonly IDistributedCache _cache;

    public GetMoviesPaginationQueryHandler(
        IMovieRepository repository,
        IMapper mapper,
        ILogger<GetMoviesPaginationQueryHandler> logger,
        IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

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

        // 1. Check Redis
        var cachedMovies = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedMovies))
        {
            _logger.LogInformation(
                "Movies page {Page} size {PageSize} found in Redis",
                request.Page,
                request.PageSize);

            return JsonSerializer.Deserialize<List<MovieResponse>>(
                cachedMovies)!;
        }

        // 2. Redis empty → PostgreSQL
        _logger.LogInformation(
            "Getting movies page {Page} size {PageSize} from database",
            request.Page,
            request.PageSize);

        var movies = await _repository.PaginationAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        // 3. Entity → Response
        var response = _mapper.Map<List<MovieResponse>>(movies);

        // 4. Response → JSON
        var json = JsonSerializer.Serialize(response);

        // 5. Save to Redis
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