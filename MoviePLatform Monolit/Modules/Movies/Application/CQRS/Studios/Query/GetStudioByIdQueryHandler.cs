using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Query;

public record GetStudioByIdQuery(int Id) : IRequest<StudioResponse>;

public class GetStudioByIdQueryHandler(
    ApplicationDbContext context,
    IMapper mapper,
    IDistributedCache cache,
    ILogger<GetStudioByIdQueryHandler> logger)
    : IRequestHandler<GetStudioByIdQuery, StudioResponse>
{
    public async Task<StudioResponse> Handle(GetStudioByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        var key = $"studio:{request.Id}";

        var cached = await cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<StudioResponse>(cached);
            if (cachedResponse is not null)
            {
                logger.LogInformation("Studio {Id} served from cache", request.Id);
                return cachedResponse;
            }
        }

        logger.LogInformation("Getting studio {Id} from database", request.Id);

        var studio = await context.Studios
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Studio with ID {request.Id} was not found");

        var response = mapper.Map<StudioResponse>(studio);

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            },
            cancellationToken);

        return response;
    }
}