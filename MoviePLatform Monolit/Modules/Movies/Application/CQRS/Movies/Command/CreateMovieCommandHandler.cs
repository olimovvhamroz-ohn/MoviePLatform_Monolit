using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;

public record CreateMovieCommand(MovieRequest Dto) : IRequest<MovieResponse>;

public class CreateMovieCommandHandler(
    IMovieRepository repo,
    IMapper mapper)
    : IRequestHandler<CreateMovieCommand, MovieResponse>
{
    public async Task<MovieResponse> Handle(CreateMovieCommand request, CancellationToken ct)
    {
        var entity = mapper.Map<MovieEntity>(request.Dto);
        var created = await repo.CreateAsync(entity, ct);


        return mapper.Map<MovieResponse>(created);
    }
}