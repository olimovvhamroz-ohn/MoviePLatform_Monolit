using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Categories.Command;
public record CreateCategoryCommand(CategoryRequest Request):IRequest<CategoryResponse>;

public class CreateCategoryCommandHandler(ILogger<CreateCategoryCommand> logger, ICategoryRepository repository,
                                                 IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    
    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating category with title: {Title}", request.Request.Title);


        var exit = await repository.ExistsByNameAsync(request.Request.Title,cancellationToken);
        
        if (exit)
        {
            logger.LogWarning("Category already exists: {Title}", request.Request.Title);
            throw new Exception(" BadRequestExceptioncategory already exists");
        }

        var model = mapper.Map<CategoryEntity>(request.Request);
        var created = await repository.CreateAsync(model,cancellationToken);

        logger.LogInformation("Category created successfully with id: {Id}", created.Id);
        return mapper.Map<CategoryResponse>(created);
    }
}