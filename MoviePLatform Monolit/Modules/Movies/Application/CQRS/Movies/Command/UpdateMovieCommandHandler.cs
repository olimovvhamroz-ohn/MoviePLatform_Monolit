using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

public record UpdateMovieCommand(
    long Id,
    MovieRequest Input) : IRequest<MovieResponse>;

public class UpdateMovieCommandHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<UpdateMovieCommandHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<UpdateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        if (request.Input == null)
            throw new BadRequestException("Input can not be null");

        if (string.IsNullOrWhiteSpace(request.Input.Title))
            throw new BadRequestException("Title is required");

        var title = request.Input.Title.Trim();

        logger.LogInformation("Updating movie {Id} via CQRS", request.Id);

        var entity = await context.Movies
                         .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                     ?? throw new NotFoundException($"Movie with ID {request.Id} was not found");

        var categoryExists = await context.Categories
            .AnyAsync(x => x.Id == request.Input.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException($"Category with id {request.Input.CategoryId} not found");

        var isDuplicate = await context.Movies
            .AnyAsync(x => x.Id != request.Id
                           && x.Title.ToLower() == title.ToLower()
                           && x.Year == request.Input.Year, cancellationToken);

        if (isDuplicate)
            throw new BadRequestException($"Movie '{title}' ({request.Input.Year}) already exists");

        entity.Title = title;
        entity.CategoryId = request.Input.CategoryId;
        entity.Year = request.Input.Year;
        entity.Price = request.Input.Price;
        entity.Description = request.Input.Description;
        entity.PosterUrl = request.Input.PosterUrl;
        entity.StudioId = request.Input.StudioId;
        entity.AgeRating = request.Input.AgeRating;

        await context.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync($"movie:{request.Id}", cancellationToken);

        return mapper.Map<MovieResponse>(entity);
    }
}