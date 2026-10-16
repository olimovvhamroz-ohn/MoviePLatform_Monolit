using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Query;

public record GetAllStudiosQuery
    : IRequest<List<StudioResponse>>;

public class GetAllStudiosQueryHandler
    (
            IStudioRepository repository,
            IMapper mapper,
            IDistributedCache cache,
            ILogger<GetAllStudiosQueryHandler> logger)
    : IRequestHandler<GetAllStudiosQuery, List<StudioResponse>>
{
   

    public async Task<List<StudioResponse>> Handle(
        GetAllStudiosQuery request,
        CancellationToken cancellationToken)
    {
        var key = "studio:all";

        // 1. Redis
        var cachedStudios = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedStudios))
        {
            logger.LogInformation(
                "Studios found in Redis cache");

            return JsonSerializer.Deserialize<List<StudioResponse>>(
                cachedStudios)!;
        }

        // 2. PostgreSQL
        logger.LogInformation(
            "Getting all studios from database");

        var studios = await repository.GetAllAsync(
            cancellationToken);

        // 3. Map
        var response = mapper.Map<List<StudioResponse>>(studios);

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