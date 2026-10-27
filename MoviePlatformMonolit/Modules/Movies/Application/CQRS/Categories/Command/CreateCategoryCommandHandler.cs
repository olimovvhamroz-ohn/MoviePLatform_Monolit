using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;


namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Categories.Command;

public record CreateCategoryCommand(CategoryRequest Request) : IRequest<CategoryResponse>;

public class CreateCategoryCommandHandler(
    ILogger<CreateCategoryCommand> logger,
    ApplicationDbContext context,
    IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.Title))
            throw new BadRequestException("Title is required");

        var title = request.Request.Title.Trim();

        logger.LogInformation("Creating category with title: {Title}", title);

        var exists = await context.Categories
            .AnyAsync(x => x.Title.ToLower() == title.ToLower(), cancellationToken);

        if (exists)
        {
            logger.LogWarning("Category already exists: {Title}", title);
            throw new BadRequestException("Category already exists");
        }

        var model = mapper.Map<CategoryEntity>(request.Request);
        model.Title = title;

        await context.Categories.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Category created successfully with id: {Id}", model.Id);
        return mapper.Map<CategoryResponse>(model);
    }
}