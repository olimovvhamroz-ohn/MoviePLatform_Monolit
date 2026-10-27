using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Categories.Query;

public record GetAllCategories : IRequest<List<CategoryResponse>>;

public class GetAllCategoriesQueryHandler(
    ApplicationDbContext context,
    ILogger<GetAllCategoriesQueryHandler> logger)
    : IRequestHandler<GetAllCategories, List<CategoryResponse>>
{
    public async Task<List<CategoryResponse>> Handle(GetAllCategories request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all categories");

        var categories = await context.Categories
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Id)
            .Select(x => new CategoryResponse
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation("Total categories found: {Count}", categories.Count);

        return categories;
    }
}