using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

public record UpdateMovieCommand(
    long Id,
    MovieRequest Input) : IRequest<MovieResponse>;

public class UpdateMovieCommandHandler(
    IMovieRepository repository,
    IMapper mapper,
    ILogger<UpdateMovieCommandHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<UpdateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(
        UpdateMovieCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating movie {Id} via CQRS",
            request.Id);

        var entity = mapper.Map<MovieEntity>(request.Input);

        var res = await repository.UpdateAsync(
            request.Id,
            entity,
            cancellationToken);

        // Remove old cache
        await cache.RemoveAsync(
            $"movie:{request.Id}",
            cancellationToken);

        return mapper.Map<MovieResponse>(res);
    }
}