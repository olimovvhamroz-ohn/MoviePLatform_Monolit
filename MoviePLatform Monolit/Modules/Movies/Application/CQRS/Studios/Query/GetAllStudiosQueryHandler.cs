using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Query;

public record GetAllStudiosQuery
    : IRequest<List<StudioResponse>>;

public class GetAllStudiosQueryHandler
    : IRequestHandler<GetAllStudiosQuery, List<StudioResponse>>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetAllStudiosQueryHandler> _logger;

    public GetAllStudiosQueryHandler(
        IStudioRepository repository,
        IMapper mapper,
        IDistributedCache cache,
        ILogger<GetAllStudiosQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<StudioResponse>> Handle(
        GetAllStudiosQuery request,
        CancellationToken cancellationToken)
    {
        var key = "studio:all";

        // 1. Redis
        var cachedStudios = await _cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedStudios))
        {
            _logger.LogInformation(
                "Studios found in Redis cache");

            return JsonSerializer.Deserialize<List<StudioResponse>>(
                cachedStudios)!;
        }

        // 2. PostgreSQL
        _logger.LogInformation(
            "Getting all studios from database");

        var studios = await _repository.GetAllAsync(
            cancellationToken);

        // 3. Map
        var response = _mapper.Map<List<StudioResponse>>(studios);

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