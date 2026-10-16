using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Categories.Command;
public record DeleteCategoriy(int Id) : IRequest<CategoryResponse>;

public class DeleteCategoriyCommandHandler(
    ILogger<DeleteCategoriyCommandHandler> logger,
    ICategoryRepository repository,
    IMapper mapper,IDistributedCache cache) : IRequestHandler<DeleteCategoriy, CategoryResponse>
{

    public async Task<CategoryResponse> Handle(DeleteCategoriy request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting category with id: {Id}", request.Id);
        var deleted = await repository.DeleteAsync(request.Id,cancellationToken);
        
        await cache.RemoveAsync(
            $"category:{request.Id}",
            cancellationToken);

        logger.LogInformation("Category deleted with id: {Id}", request.Id);
        
        return mapper.Map<CategoryResponse>(deleted);
    }
}