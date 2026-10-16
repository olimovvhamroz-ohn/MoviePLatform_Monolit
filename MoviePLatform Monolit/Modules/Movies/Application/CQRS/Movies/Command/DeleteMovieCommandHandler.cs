using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

public record DeleteMovieCommand(long Id) : IRequest<MovieResponse>;

public class DeleteMovieCommandHandler(
    IMovieRepository repository,
    IMapper mapper,
    ILogger<DeleteMovieCommandHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<DeleteMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(
        DeleteMovieCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Deleting movie {Id} via CQRS",
            request.Id);

        var res = await repository.DeleteAsync(
            request.Id,
            cancellationToken);

        await cache.RemoveAsync(
            $"movie:{request.Id}",
            cancellationToken);

        return mapper.Map<MovieResponse>(res);
    }
}