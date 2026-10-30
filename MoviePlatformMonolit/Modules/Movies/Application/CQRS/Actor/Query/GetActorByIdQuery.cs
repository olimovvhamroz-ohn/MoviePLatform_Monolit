using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Query;

public record GetByIdActorQuery(long ActorId) : IRequest<ActorResponse>;

public class GetActorByIdQueryHandler(
    ApplicationDbContext context,
    IDistributedCache cache)
    : IRequestHandler<GetByIdActorQuery, ActorResponse>
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
    };

    public async Task<ActorResponse> Handle(GetByIdActorQuery request, CancellationToken cancellationToken)
    {
        if (request.ActorId <= 0)
            throw new BadRequestException("ID must be greater than zero");

        var cacheKey = $"actor:{request.ActorId}";

        var cachedActor = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (cachedActor is not null)
            return JsonSerializer.Deserialize<ActorResponse>(cachedActor)!;

        var actor = await context.Actors
                        .AsNoTracking()
                        .Where(x => !x.IsDeleted && x.Id == request.ActorId)
                        .Select(x => new ActorResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Biography = x.Biography
                        })
                        .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new NotFoundException($"Actor with id {request.ActorId} not found");

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(actor),
            CacheOptions,
            cancellationToken);

        return actor;
    }
}