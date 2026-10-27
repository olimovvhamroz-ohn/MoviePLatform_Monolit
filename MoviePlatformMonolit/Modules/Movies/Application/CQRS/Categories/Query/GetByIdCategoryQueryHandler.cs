//using System.Text.Json;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Categories.Query;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponse>;

public class GetCategoryByIdQueryHandler(
    ILogger<GetCategoryByIdQueryHandler> logger,
    ApplicationDbContext context,
    IMapper mapper
    //,IDistributedCache cache
) : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0) throw new BadRequestException("ID must be greater than zero");

        /*var key = $"category:{request.Id}";
        var cached = await cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<CategoryResponse>(cached);
            if (cachedResponse is not null)
            {
                logger.LogInformation("Category {Id} served from cache", request.Id);
                return cachedResponse;
            }
        }
*/
        logger.LogInformation("Getting category by id: {Id} via CQRS", request.Id);


        var res =
            await context.Categories
                      .Where(x => x.IsDeleted == false)
                      .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                  ?? throw new NotFoundException($"Id {request.Id} not found");
        
        
        logger.LogInformation("Category found: {Title}", res.Title);

        var response = mapper.Map<CategoryResponse>(res);


        /* var json = JsonSerializer.Serialize(response);

         await cache.SetStringAsync(
             key,
             json,
             new DistributedCacheEntryOptions
             {
                 AbsoluteExpirationRelativeToNow =
                     TimeSpan.FromMinutes(10)
             },
             cancellationToken);
         logger.LogInformation("Category {Id} loaded", request.Id);*/
        return response;
    }
}