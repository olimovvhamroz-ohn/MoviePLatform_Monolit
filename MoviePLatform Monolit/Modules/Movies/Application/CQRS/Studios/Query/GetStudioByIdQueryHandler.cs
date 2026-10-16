using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Query;

public record GetStudioByIdQuery(int Id) : IRequest<StudioResponse>;

public class GetStudioByIdQueryHandler
 (
         IStudioRepository repository,
         IMapper mapper,
         IDistributedCache cache,
         ILogger<GetStudioByIdQueryHandler> logger)  
         : IRequestHandler<GetStudioByIdQuery, StudioResponse>
{
  

    public async Task<StudioResponse> Handle(
        GetStudioByIdQuery request,
        CancellationToken cancellationToken)
    {
        var key = $"studio:{request.Id}";

        // 1. Check Redis
        var cachedStudio = await cache.GetStringAsync(
            key,
            cancellationToken);

        if (!string.IsNullOrEmpty(cachedStudio))
        {
            logger.LogInformation(
                "Studio {Id} found in Redis cache",
                request.Id);

            return JsonSerializer.Deserialize<StudioResponse>(
                cachedStudio)!;
        }

        // 2. Redis empty → PostgreSQL
        logger.LogInformation(
            "Getting studio {Id} from database",
            request.Id);

        var studio = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        // 3. Entity → Response
        var response = mapper.Map<StudioResponse>(studio);

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