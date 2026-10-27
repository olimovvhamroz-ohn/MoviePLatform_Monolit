//using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Studios.Query;

public record GetAllStudiosQuery : IRequest<List<StudioResponse>>;

public class GetAllStudiosQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    //IDistributedCache cache,
    ILogger<GetAllStudiosQueryHandler> logger)
    : IRequestHandler<GetAllStudiosQuery, List<StudioResponse>>
{
    //private const string CacheKey = "studio:all";

    public async Task<List<StudioResponse>> Handle(GetAllStudiosQuery request, CancellationToken cancellationToken)
    {
        /*var cached = await cache.GetStringAsync(CacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<List<StudioResponse>>(cached);
            if (cachedResponse is not null)
            {
                logger.LogInformation("Studios served from cache");
                return cachedResponse;
            }
        }*/

        logger.LogInformation("Getting all studios from database");

        var studios = await context.Studios
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var response = mapper.Map<List<StudioResponse>>(studios);
/*
        await cache.SetStringAsync(
            CacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);
  */

        return response;
    }
}