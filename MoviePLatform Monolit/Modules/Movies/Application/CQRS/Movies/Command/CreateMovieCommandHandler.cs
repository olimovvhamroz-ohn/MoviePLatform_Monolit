using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

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

        var categoryExists = await context.Categories
            .AnyAsync(x => x.Id == request.Request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new NotFoundException($"Category with id {request.Request.CategoryId} not found");

        var isDuplicate = await context.Movies
            .AnyAsync(x => x.Title.ToLower() == title.ToLower() && x.Year == request.Request.Year, cancellationToken);

        if (isDuplicate)
            throw new BadRequestException($"Movie '{title}' ({request.Request.Year}) already exists");

        var entity = mapper.Map<MovieEntity>(request.Request);
        entity.Title = title;

        await context.Movies.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<MovieResponse>(entity);
    }
}