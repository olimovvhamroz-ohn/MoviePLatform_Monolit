using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Command;

public record CreateMovieCommand(MovieRequest Request) : IRequest<MovieResponse>;

public class CreateMovieCommandHandler(
    ApplicationDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.Title))
            throw new BadRequestException("Title is required");

        var title = request.Request.Title.Trim();
         var titleLower=title.ToLower();
        var categoryExists = await context.Categories
            .AnyAsync(x =>!x.IsDeleted && x.Id == request.Request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException($"Category with id {request.Request.CategoryId} not found");

        var isDuplicate = await context.Movies
            .AnyAsync(x => !x.IsDeleted
                           && x.Title.ToLower() == titleLower
                                        && x.Year == request.Request.Year, cancellationToken);

        if (isDuplicate)
            throw new BadRequestException($"Movie '{title}' ({request.Request.Year}) already exists");

        var entity = mapper.Map<MovieEntity>(request.Request);
        entity.Title = title;

        await context.Movies.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await context.Movies
            .AsNoTracking()
            .Where(x => x.Id == entity.Id)
            .Select(MovieProjections.ToResponse)
            .FirstAsync(cancellationToken);
    }    
}