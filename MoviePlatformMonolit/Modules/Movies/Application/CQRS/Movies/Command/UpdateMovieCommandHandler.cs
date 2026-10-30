using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Command;

public record UpdateMovieCommand(long Id, MovieRequest Input) : IRequest<MovieResponse>;

public class UpdateMovieCommandHandler(
    ApplicationDbContext context,
    IDistributedCache cache,
    ILogger<UpdateMovieCommandHandler> logger)
    : IRequestHandler<UpdateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        if (request.Input is null)
            throw new BadRequestException("Input can not be null");

        if (string.IsNullOrWhiteSpace(request.Input.Title))
            throw new BadRequestException("Title is required");

        var input = request.Input;
        var title = input.Title.Trim();
        var titleLower = title.ToLower();

        logger.LogInformation("Updating movie {Id}", request.Id);

        var entity = await context.Movies
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Movie with ID {request.Id} was not found");

        var categoryExists = await context.Categories
            .AnyAsync(x => !x.IsDeleted && x.Id == input.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException($"Category with id {input.CategoryId} not found");

        var isDuplicate = await context.Movies
            .AnyAsync(x => !x.IsDeleted
                           && x.Id != request.Id
                           && x.Title.ToLower() == titleLower
                           && x.Year == input.Year, cancellationToken);

        if (isDuplicate)
            throw new BadRequestException($"Movie '{title}' ({input.Year}) already exists");

        entity.Title = title;
        entity.CategoryId = input.CategoryId;
        entity.Year = input.Year;
        entity.Price = input.Price;
        entity.Description = input.Description;
        entity.PosterUrl = input.PosterUrl;
        entity.StudioId = input.StudioId;
        entity.AgeRating = input.AgeRating;

        await context.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync($"movie:{request.Id}", cancellationToken);
        

        return await context.Movies
            .AsNoTracking()
            .Where(x => x.Id == entity.Id)
            .Select(MovieProjections.ToResponse)
            .FirstAsync(cancellationToken);
    }
}