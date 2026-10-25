using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Categories.Command;

public record DeleteCategoryCommand(int Id) : IRequest<CategoryResponse>;

public class DeleteCategoryCommandHandler(
    ILogger<DeleteCategoryCommandHandler> logger,
    ApplicationDbContext context,
    IMapper mapper,
    IDistributedCache cache) : IRequestHandler<DeleteCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0) throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation("Deleting category with id: {Id}", request.Id);

        var res = await context.Categories.Where(x => x.IsDeleted == false)
                      .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                  ?? throw new NotFoundException($"Category with id {request.Id} not found");

        res.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(
            $"category:{request.Id}",
            cancellationToken);

        logger.LogInformation("Category deleted with id: {Id}", request.Id);

        return mapper.Map<CategoryResponse>(res);
    }
}