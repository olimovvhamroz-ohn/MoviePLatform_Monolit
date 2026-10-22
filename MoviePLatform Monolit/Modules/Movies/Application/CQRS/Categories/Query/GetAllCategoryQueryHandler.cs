using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Categories.Query;

public record GetAllCategories : IRequest<List<CategoryResponse>>;

public class GetAllCatecoriyQuery(
    ILogger<GetAllCatecoriyQuery> logger,
    ApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<GetAllCategories, List<CategoryResponse>>
{
    public async Task<List<CategoryResponse>> Handle(GetAllCategories request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all categories");

        var data = await context.Categories.AsNoTracking().ToListAsync(cancellationToken);
        
        logger.LogInformation("Total categories found: {Count}", data.Count);

        return mapper.Map<List<CategoryResponse>>(data);
    }
}