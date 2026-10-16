using System.Text.Json;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Categories.Query;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponse>;

public class GetCategoryByIdQueryHandler(
    ILogger<GetCategoryByIdQueryHandler> logger,
    ICategoryRepository repository,
    IMapper mapper,
    IDistributedCache cache) : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var key = $"category:{request.Id}";
        var cach = await cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(cach))
        {
            logger.LogInformation("Category found with id: {Id}", request.Id);
            return JsonSerializer.Deserialize<CategoryResponse>(cach)!;
        }

        logger.LogInformation("Getting category by id: {Id} via CQRS", request.Id);

        var data = await repository.GetByIdAsync(request.Id, cancellationToken);

        logger.LogInformation("Category found: {Title}", data.Title);

        var response = mapper.Map<CategoryResponse>(data);
        var json = JsonSerializer.Serialize(response);

        await cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10)
            },
            cancellationToken);
        logger.LogInformation("Category {Id} loaded", request.Id);
        return response;
    }
}